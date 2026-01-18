using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Features.Features.Shared.Responses;

namespace Modules.Workflows.Features.Features.GetActiveWorkflows;



internal interface IGetActiveWorkflowsHandler : IHandler
{
	Task<Result<List<WorkflowResponse>>> HandleAsync(CancellationToken cancellationToken);
}


internal sealed class GetActiveWorkflowsHandler(
	ILogger<GetActiveWorkflowsHandler> logger,
	ILinkService linkService) : IGetActiveWorkflowsHandler
{
	public async Task<Result<List<WorkflowResponse>>> HandleAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

		// Implementation goes here

		const string workflowCode = "123";
		const string type = "Verify";
		var nextStepLink = linkService.Generate("WorkflowNextStep", new { code = workflowCode, type }, "Move to Next Step", "PATCH");
		
		var wf = new WorkflowResponse(workflowCode, "Приемка по накладной", "Приемка по каждой строчки накладной")
		{
			CurrentStep = new WorkflowCurrentStep("Scan", "Шаг сканирования", "")
			{
				Actions = new Actions
				{
					Links = new List<Link>
					{
						new Link("/api/item","Increment Qty","POST"),
						new Link("/api/", "Add Line", "PUT")
					},
					NextStep = nextStepLink
				},

			},
			WorkflowSteps = new List<WorkflowStepShortResponse>
			{
				new WorkflowStepShortResponse("Scan", "Шаг сканирования", 1),
				new WorkflowStepShortResponse("Verify", "Шаг проверки", 2),
				new WorkflowStepShortResponse("Complete", "Шаг завершения", 3)
			}
		};

		var response = new List<WorkflowResponse>() { wf };

		return response;
	}
}
