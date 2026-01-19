using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Features.Features.Shared.Responses;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.GetActiveWorkflows;



internal interface IGetActiveWorkflowsHandler : IHandler
{
	Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken);
}


internal sealed class GetActiveWorkflowsHandler(
	ILogger<GetActiveWorkflowsHandler> logger,
	ILinkService linkService) : IGetActiveWorkflowsHandler
{
	public async Task<Result<List<WorkflowShortInfoResponse>>> HandleAsync(string workflowTypeCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

		// Implementation goes here
		const string workflowCode = "123";
		
		var getWorkflowLink = linkService.Generate("GetWorkflow", new { code = workflowCode }, $"Get {workflowTypeCode} workflow data", HttpMethod.GET);

		var wf = new WorkflowShortInfoResponse(workflowCode, workflowTypeCode, "Приемка по накладной", "Приемка по каждой строчки накладной")
		{
			CurrentStepName = "Шаг сканирования",
			CurrentStepType = "Scan",
			Links = new List<Link> { getWorkflowLink },
			Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
			DataSchema = new WorkflowStepDataSchema
			{
				Version = "1.0",
				DataType = "ReceiveGoods",
				SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
			},


		};

		var response = new List<WorkflowShortInfoResponse>() { wf };

		return response;
	}

	
}
