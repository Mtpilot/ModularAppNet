using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Mappers;
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

		var response = workflow.ToResponse(JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement, new WorkflowStepDataSchema
        {
            Version = "1.0",
            DataType = workflow.TypeCode,
            SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
        }, linkService);
		return response;
	}

	private static string GenerateWorkflowCode()
	{
		// Simple code generation - in real implementation might use more sophisticated approach
		return Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
	}

	private static Workflow<IDataItem> BuildWorkflow(string code, CreateWorkflowRequest request)
	{
		var id =  Guid.NewGuid();
        var firstStep = GetFirstStepForType(id, request.TypeCode);
		
		return new Workflow<IDataItem>
		{	
			Id= id,
			WorkflowDataItems = new List<WorkflowDataItem>(),
			Code = code,
			TypeCode = request.TypeCode,
			Name = request.Name,
			Description = request.Description,
			IsActive = true,
			CurrentStepType = firstStep.Type,
            CurrentStepNumber = 1,
            Data = new WorkflowDataItemsCollection<IDataItem>
			{
				Name = "Workflow Data",
				Description = "Workflow data collection",
				Collection = new List<IDataItem>()
			},
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
		return new List<WorkflowStep>
		{
			new WorkflowStep
			{
				Id = Guid.NewGuid(),
				WorkflowId = workflowId,
				Type = "Scan",
				Name = "Шаг сканирования",
				Description = "Сканирование товара",
				Order = 1,
				Actions = new List<WorkflowStepAction>()
			},
			new WorkflowStep
			{
				Id = Guid.NewGuid(),
				WorkflowId = workflowId,
				Type = "Verify",
				Name = "Шаг проверки",
				Description = "Проверка количества",
				Order = 2,
				Actions = new List<WorkflowStepAction>()
			},
			new WorkflowStep
			{
				Id = Guid.NewGuid(),
				WorkflowId= workflowId,
				Type = "Accept",
				Name = "Шаг приемки",
				Description = "Подтверждение приемки",
				Order = 3,
				Actions = new List<WorkflowStepAction>()
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
