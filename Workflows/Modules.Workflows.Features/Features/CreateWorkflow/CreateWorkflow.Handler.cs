using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.CreateWorkflow;

internal interface ICreateWorkflowHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(CreateWorkflowRequest request, CancellationToken cancellationToken);
}

internal sealed class CreateWorkflowHandler(
	WorkflowsDbContext context,
	ILogger<CreateWorkflowHandler> logger,
	ILinkService linkService) : ICreateWorkflowHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(CreateWorkflowRequest request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Creating workflow with type '{TypeCode}' and name '{Name}'", request.TypeCode, request.Name);

		// TODO: Check if workflow with same code already exists (when real storage is implemented)
		// TODO: Load workflow type configuration to get steps
		// For now, using sample implementation

		var workflowCode = GenerateWorkflowCode();
		
		// Build workflow with first step
		var workflow = BuildWorkflow(workflowCode, request);

		await context.Workflows.AddAsync(workflow, cancellationToken); //SESZH: немного влез в создание
        await context.SaveChangesAsync(cancellationToken);


        // TODO: Save workflow to storage (database, cache, etc.)

        logger.LogInformation("Created workflow with code '{Code}'", workflowCode);

		var response = workflow.ToPartialResponse(new WorkflowStepDataSchema
        {
            Version = "1.0",
            DataType = workflow.Type.Code,
            SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
        }, linkService);
		return response;
	}

	private static string GenerateWorkflowCode()
	{
		// Simple code generation - in real implementation might use more sophisticated approach
		return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
	}

	private static Workflow BuildWorkflow(string workflowCode, CreateWorkflowRequest request)
	{
		var id =  Guid.NewGuid();
        var firstStep = GetFirstStepForType(id, request.TypeCode);
		
		return new Workflow
		{
			Id = id,
			Code = workflowCode,
			Type = new WorkflowType(request.TypeCode, request.Name) { Id = Guid.NewGuid() },
			Name = request.Name,
			Description = request.Description,
			IsActive = true,
			CurrentStepType = firstStep.Type,
			CurrentStepNumber = 1,
			Data = new DefaultWorkflowDataCollection<InvoiceHeader>
			{
				Name = "Новое Воркфлоу",
				Description = "",
				Collection = new List<InvoiceHeader>
				{
					new InvoiceHeader
					{
						WorkflowId = id,
						ContractNumber = "Новая накладная",
						Counterparty = "Новый источник",
						InvoiceNumber = $"Новый Айдишник: {Guid.NewGuid().ToString()}",
						Date = DateTime.UtcNow,
					}
				}
			},
			//Data = new WorkflowDataItemsCollection<IDataItem>
			//{
			//	Name = "Workflow Data",
			//	Description = "Workflow data collection",
			//	Collection = new List<IDataItem>()
			//},
			Steps = GetStepsForType(id, request.TypeCode)
		};
	}

	private static WorkflowStep GetFirstStepForType(Guid id, string typeCode)
	{
		// Sample implementation - in real app, this would load from workflow type configuration
		var steps = GetStepsForType(id, typeCode);
		return steps.OrderBy(s => s.Order).First();
	}

	private static List<WorkflowStep> GetStepsForType(Guid workflowId, string typeCode)
	{
		// Sample implementation - in real app, this would load from workflow type configuration based on typeCode
		_ = typeCode; // Will be used in real implementation
		var step1Id = Guid.NewGuid();
		var step2Id = Guid.NewGuid();
		var step3Id = Guid.NewGuid();
		return new List<WorkflowStep>
		{
			new WorkflowStep
			{
				Id = step1Id,
				WorkflowId = workflowId,
				StepCode = "Scan-01",
				Type = WorkflowStepType.Scan,
				Name = "Шаг сканирования",
				Description = "Сканирование товара",
				Order = 1,
				Actions = new List<WorkflowStepAction>(),
				Data = new DefaultWorkflowDataCollection<Invoice>
				{
					Name = "Сканированные товары",
					Description = "Коллекция отсканированных товаров",
					Collection = new List<Invoice>
					{
						new Invoice
						{
							Date = DateTime.UtcNow,
							WorkflowId = workflowId,
							StepId = step1Id,
							//Name = "Накладная 1",
							ContractNumber = "Контракт 1",
							Counterparty = "Поставщик 1",
							InvoiceNumber = "ID родительского",
							Lines = new List<InvoiceLine>
							{
								new InvoiceLine
								{
									ProductName = "Товар 1",
									ConstructorName = "Конструкторское имя 1",
									ProductCode = "Код товара 1",
									Quantity = 10,
									Units = "шт",
									Barcode = "1234567890123"
                                },
                            }
                        }
					}
                }
			},
			new WorkflowStep
			{
				Id = step2Id,
				WorkflowId = workflowId,
				StepCode = "Verify-02",
				Type = WorkflowStepType.Verify,
				Name = "Шаг проверки",
				Description = "Проверка количества",
				Order = 2,
				Actions = new List<WorkflowStepAction>(),
                Data = new DefaultWorkflowDataCollection<Invoice>
                {
                    Name = "Сканированные товары",
                    Description = "Коллекция отсканированных товаров",
                    Collection = new List<Invoice>(),
                }
            },
			new WorkflowStep
			{
				Id = step3Id,
				WorkflowId= workflowId,
				StepCode = "Accept-03",
				Type = WorkflowStepType.Accept,
				Name = "Шаг приемки",
				Description = "Подтверждение приемки",
				Order = 3,
				Actions = new List<WorkflowStepAction>(),
				Data = new DefaultWorkflowDataCollection<Invoice>
                {
                    Name = "Сканированные товары",
                    Description = "Коллекция отсканированных товаров",
                    Collection = new List<Invoice>(),
                }
            }
		};
	}

	//private static WorkflowResponse MapToResponse(Workflow<Dictionary<string, object>> workflow, ILinkService linkService)
	//{
	//	var currentStep = workflow.CurrentStep();
	//	var nextStep = workflow.GetNextStep(currentStep.Order);
	//	
	//	var nextStepLink = nextStep != null
	//		? linkService.Generate("WorkflowNextStep", new { code = workflow.Code, stepType = nextStep.Type }, "Move to Next Step", HttpMethod.PATCH)
	//		: null;
	//
	//	return new WorkflowResponse(workflow.Code, workflow.TypeCode, workflow.Name, workflow.Description)
	//	{
	//		Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
	//		DataSchema = new WorkflowStepDataSchema
	//		{
	//			Version = "1.0",
	//			DataType = workflow.TypeCode,
	//			SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
	//		},
	//
	//		CurrentStep = new WorkflowCurrentStep(currentStep.Type, currentStep.Name, currentStep.Description)
	//		{
	//			Actions = new WorkflowActions
    //            {
    //                StepActions = new List<Link>
	//				{
	//					new Link("/api/item", "Increment Qty", HttpMethod.POST),
	//					new Link("/api/", "Add Line", HttpMethod.PUT)
	//				},
	//				NextStep = nextStepLink
	//			},
	//			DataSchema = new WorkflowStepDataSchema
	//			{
	//				Version = "1.0",
	//				DataType = "Invoice",
	//				SchemaJson = "{ 'type': 'object', 'properties': { 'invoiceNumber': { 'type': 'string' }, 'items': { 'type': 'array', 'items': { 'type': 'object', 'properties': { 'itemCode': { 'type': 'string' }, 'quantity': { 'type': 'integer' } }, 'required': ['itemCode', 'quantity'] } } }, 'required': ['invoiceNumber', 'items'] }",
	//			},
	//			Data = JsonDocument.Parse("{}").RootElement
	//		},
	//		WorkflowSteps = workflow.Steps
	//			.OrderBy(s => s.Order)
	//			.Select(s => new WorkflowStepResponse(s.Type, s.Name, s.Order, s.Description))
	//			.ToList()
	//	};
	//}
	//
	//private sealed class WorkflowDataCollection : IWorkflowDataCollection<Dictionary<string, object>>
	//{
	//	public string Name { get; set; } = "Data";
	//	public string Description { get; set; } = "Workflow data";
	//	public ICollection<Dictionary<string, object>> Collection { get; set; } = new List<Dictionary<string, object>>();
	//}
}
