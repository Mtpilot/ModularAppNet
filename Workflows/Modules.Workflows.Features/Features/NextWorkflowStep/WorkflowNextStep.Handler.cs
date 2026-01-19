using System.Text.Json;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Responses;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

internal sealed record WorkflowNextStepCommand(string Code, string StepType, JsonElement Data);

internal interface IWorkflowNextStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken);
}

internal sealed class WorkflowNextStepHandler(ILogger<WorkflowNextStepHandler> logger, ILinkService linkService) : IWorkflowNextStepHandler

{
#pragma warning disable MA0051 // Method is too long
	public Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken)
#pragma warning restore MA0051 // Method is too long
	{
		logger.LogInformation("Advancing workflow {WorkflowCode} from step {StepType}", request.Code, request.StepType);

		var workflow = BuildSampleWorkflow(); //TODO: поднимаем вокрфлоу по коду для перевода на следующий шаг
		if (!workflow.Code.Equals(request.Code, StringComparison.OrdinalIgnoreCase))
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.NotFound(request.Code));
		}

		var currentStep = workflow.CurrentStep();
		if (currentStep is null)
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.StepNotFound(request.StepType));
		}

		var nextStep = workflow.GetNextStep(currentStep.Order);
		if (nextStep is null)
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.NextStepNotFound(request.Code, request.StepType));
		}

		if (!nextStep.Type.Equals(request.StepType, StringComparison.OrdinalIgnoreCase))
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.StepMissMatch(request.Code, nextStep.Type, request.StepType));
		}

		var nextAvailableStep = workflow.GetNextStep(nextStep.Order);
		const string workflowTypeCode = "ReceiveGoods";
		var response = new WorkflowResponse(workflow.Code, workflowTypeCode, workflow.Name, workflow.Description)
		{
			Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
			DataSchema = new WorkflowStepDataSchema
			{
				Version = "1.0",
				DataType = "ReceiveGoods",
				SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
			},
			CurrentStep = new WorkflowCurrentStep(nextStep.Type, nextStep.Name, "Step Description")
			{
				Actions = new Actions
				{
					Links = new List<Link>
					{
						new Link("/api/item","Increment Qty", HttpMethod.POST),
						new Link("/api/", "Add Line", HttpMethod.PUT)
					},
					NextStep = nextAvailableStep != null ? linkService.Generate("WorkflowNextStep", new { code = workflow.Code, nextAvailableStep.Type }, "Move to Next Step", HttpMethod.PATCH) : null
				},
				DataSchema = new WorkflowStepDataSchema
				{
					Version = "1.0",
					DataType = "Invoice",
					SchemaJson = "{ 'type': 'object', 'properties': { 'invoiceNumber': { 'type': 'string' }, 'items': { 'type': 'array', 'items': { 'type': 'object', 'properties': { 'itemCode': { 'type': 'string' }, 'quantity': { 'type': 'integer' } }, 'required': ['itemCode', 'quantity'] } } }, 'required': ['invoiceNumber', 'items'] }",
				},
				// Example: Use data from request or create sample data
				Data = request.Data.ValueKind != JsonValueKind.Null && request.Data.ValueKind != JsonValueKind.Undefined 
					? request.Data.Clone() 
					: CreateSampleData()
			},

			WorkflowSteps = new List<WorkflowStepShortResponse>
			{
				new WorkflowStepShortResponse("Scan", "Шаг сканирования", 1),
				new WorkflowStepShortResponse("Verify", "Шаг проверки", 2),
				new WorkflowStepShortResponse("Complete", "Шаг завершения", 3)
			}
		};

		return Task.FromResult<Result<WorkflowResponse>>(response);
	}


	//This is a sample workflow for demonstration purposes.
	
	private static Workflow<Dictionary<string, object>> BuildSampleWorkflow()
	{
		const string currentStepType = "Scan";
		const string workflowTypeCode = "ReceiveGoods";
		return new Workflow<Dictionary<string, object>>
		{
			Code = "123",
			TypeCode = workflowTypeCode,
			Name = "Приемка по накладной",
			Description = "Приемка по каждой строчки накладной",
			IsActive = true,
			Data = new WorkflowDataCollection(),
			CurrentStepType = currentStepType,
			Steps =
			[
				new WorkflowStep
				{
					Type = currentStepType,
					Name = "Шаг сканирования",
					Description = "Сканирование товара",
					Order = 1,
					Actions = []
				},
				new WorkflowStep
				{
					Type = "Verify",
					Name = "Шаг проверки",
					Description = "Проверка количества",
					Order = 2,
					Actions = []
				},
				new WorkflowStep
				{
					Type = "Accept",
					Name = "Шаг приемки",
					Description = "Подтверждение приемки",
					Order = 3,
					Actions = []
				}
			]
		};
	}

	private sealed class WorkflowDataCollection : IWorkflowDataCollection<Dictionary<string, object>>
	{
		public string Name { get; set; } = "Data";
		public string Description { get; set; } = "Workflow data";
		public ICollection<Dictionary<string, object>> Collection { get; set; } = new List<Dictionary<string, object>>();
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
