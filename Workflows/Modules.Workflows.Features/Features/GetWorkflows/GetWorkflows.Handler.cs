using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Infrastructure.Helpers;
using Modules.Workflows.MockInfrastructure.Database;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.GetWorkflows;

internal interface IGetWorkflowsHandler : IHandler
{
	Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken);
}

internal sealed class GetWorkflowsHandler(
	ILogger<GetWorkflowsHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService) : IGetWorkflowsHandler
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
            var tmpData =  await MockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(context, workflow.Id, cancellationToken);

			workflow.Data = tmpData;
            var getWorkflowLink = linkService.Generate("GetWorkflow", new { workflowCode = workflow.Code }, $"Get {workflowTypeCode} workflow data", HttpMethod.GET);

			var currentStep = workflow.CurrentStep();

			var wf = new WorkflowShortInfoResponse(workflow.Code, workflowTypeCode, workflow.Name, workflow.Description)
			{
				CurrentStepName = currentStep?.Name ?? string.Empty,
				CurrentStepType = workflow.CurrentStepType,
				Links = new List<Link> { getWorkflowLink },
				Data = JsonSerializer.Serialize(tmpData), //TODO: надо серелизовать из workflow.Data (но из отсутвия Generic нужно подумать как лучше его подставлять) 
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
