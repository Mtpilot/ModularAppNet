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

namespace Modules.Workflows.Features.Features.GetWorkflow;

internal interface IGetWorkflowHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string code, CancellationToken cancellationToken);
}


internal sealed class GetWorkflowHandler(
	ILogger<GetWorkflowHandler> logger,
	ILinkService linkService) : IGetWorkflowHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string code, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

		// Implementation goes here
		const string workflowTypeCode = "ReceiveGoods";
		const string workflowCode = "123";
		const string type = "Verify";
		var nextStepLink = linkService.Generate("WorkflowNextStep", new { code = workflowCode, type }, "Move to Next Step", HttpMethod.PATCH);

		var wf = new WorkflowResponse(workflowCode, workflowTypeCode, "Приемка по накладной", "Приемка по каждой строчки накладной")
		{
			Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
			DataSchema = new WorkflowStepDataSchema
			{
				Version = "1.0",
				DataType = "ReceiveGoods",
				SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
			},

			CurrentStep = new WorkflowCurrentStep("Scan", "Шаг сканирования", "")
			{
			Actions = new Actions
			{
				Links = new List<Link>
				{
					new Link("/api/item","Increment Qty", HttpMethod.POST),
					new Link("/api/", "Add Line", HttpMethod.PUT)
				},
				NextStep = nextStepLink
			},
				DataSchema = new WorkflowStepDataSchema
				{
					Version = "1.0",
					DataType = "Invoice",
					SchemaJson = "{ 'type': 'object', 'properties': { 'invoiceNumber': { 'type': 'string' }, 'items': { 'type': 'array', 'items': { 'type': 'object', 'properties': { 'itemCode': { 'type': 'string' }, 'quantity': { 'type': 'integer' } }, 'required': ['itemCode', 'quantity'] } } }, 'required': ['invoiceNumber', 'items'] }",
				},
				Data = CreateSampleData()

			},
			WorkflowSteps = new List<WorkflowStepShortResponse>
			{
				new WorkflowStepShortResponse("Scan", "Шаг сканирования", 1),
				new WorkflowStepShortResponse("Verify", "Шаг проверки", 2),
				new WorkflowStepShortResponse("Complete", "Шаг завершения", 3)
			}
		};


		return wf;
	}

	/// <summary>
	/// Example of creating JsonElement with sample data for CurrentStep.Data field.
	/// This demonstrates how to populate Data with arbitrary JSON structures including arrays and nested objects.
	/// </summary>
	private static JsonElement CreateSampleData()
	{
		var sampleData = new
		{
			invoiceNumber = "INV-2024-001",
			date = "2024-01-15",
			supplier = new
			{
				name = "ООО Поставщик",
				inn = "1234567890"
			},
			items = new[]
			{
				new
				{
					itemCode = "PROD-001",
					name = "Товар 1",
					quantity = 10,
					unit = "шт",
					price = 1500.50m,
					tags = new[] { "urgent", "fragile" }
				},
				new
				{
					itemCode = "PROD-002",
					name = "Товар 2",
					quantity = 5,
					unit = "шт",
					price = 2300.00m,
					tags = new[] { "standard" }
				}
			},
			metadata = new
			{
				source = "warehouse",
				priority = "high",
				notes = new[]
				{
					"Требуется проверка качества",
					"Срочная доставка"
				}
			}
		};

		var jsonString = JsonSerializer.Serialize(sampleData);
		using var document = JsonDocument.Parse(jsonString);
		return document.RootElement.Clone();
	}
}
