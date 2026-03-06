using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using System.Text.Json;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.GetWorkflows;

internal interface IGetWorkflowsHandler : IHandler
{
	Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken);
}

internal sealed class GetWorkflowsHandler(
	ILogger<GetWorkflowsHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService,
    IInMemoryDbHelper mockTmpHelper) : IGetWorkflowsHandler
{
	public async Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting workflows");

		var workflows = await context.Workflows
			.Include(x => x.Type)
			.Include(x => x.Steps).ThenInclude(s => s.Actions)
			.Where(x => x.Type.Code == workflowTypeCode).ToListAsync(cancellationToken);

		var response = new List<WorkflowShortInfoResponse>();

		
        foreach (var workflow in workflows)
		{
            var workflowData = new WorkflowDataCollection<IBaseWorkflowDataDto> { Name = workflow.Name, Description = workflow.Description, Collection = await mockTmpHelper.GetWorkflowDataFromInMemoryDb(workflow.Id, cancellationToken) };

            var getWorkflowLink = linkService.Generate(EndpointConsts.GetWorkflow, new { workflowCode = workflow.Code }, $"Get {workflowTypeCode} workflow data", HttpMethod.GET);

			var currentStep = workflow.CurrentStep();

			var wf = new WorkflowShortInfoResponse(workflow.Code, workflowTypeCode, workflow.Name, workflow.Description, workflow.IsActive)
			{
				CurrentStepName = currentStep?.Name ?? string.Empty,
				CurrentStepType = workflow.GetCurrentStepType(),
				Links = new List<Link> { getWorkflowLink },
				Data = WorkflowDataCollectionSerializer.Serialize(workflowData),
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
