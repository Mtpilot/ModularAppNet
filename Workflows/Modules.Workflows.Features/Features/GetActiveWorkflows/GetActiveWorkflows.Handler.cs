using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using System.Text.Json;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Features.Features.GetActiveWorkflows;



internal interface IGetActiveWorkflowsHandler : IHandler
{
	Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken);
}


internal sealed class GetActiveWorkflowsHandler(
	WorkflowsDbContext context,
	ILogger<GetActiveWorkflowsHandler> logger,
	ILinkService linkService,
    IMockTmpHelper mockTmpHelper) : IGetActiveWorkflowsHandler
{
	public async Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

        var workflows = await context.Workflows
            .Include(x => x.Type)
            .Include(x => x.Steps).ThenInclude(s => s.Actions)
            .Where(x => x.Type.Code == workflowTypeCode && x.IsActive).ToListAsync(cancellationToken);

        var response = new List<WorkflowShortInfoResponse>();
        foreach (var workflow in workflows)
        {
            workflow.Data = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = "InvoiceHeaders", Description = "Collection of invoice headers", Collection = await mockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(workflow.Id, cancellationToken) };
          
            var getWorkflowLink = linkService.Generate("GetWorkflow", new { workflowCode = workflow.Code }, $"Get {workflowTypeCode} workflow data", HttpMethod.GET);

            var currentStep = workflow.CurrentStep();
            

            var wf = new WorkflowShortInfoResponse(workflow.Code, workflowTypeCode, workflow.Name, workflow.Description, workflow.IsActive)
            {
                CurrentStepName = currentStep?.Name ?? string.Empty,
                CurrentStepType = workflow.CurrentStepType.ToString(),
                Links = new List<Link> { getWorkflowLink },
                Data = JsonSerializer.Serialize(workflow.Data),
                DataSchema = new WorkflowStepDataSchema
                {
                    Version = "1.0",
                    DataType = "ReceiveGoods",
                    SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
                },
            };
            response.Add(wf);
        }

        return response;
	}

	
}
