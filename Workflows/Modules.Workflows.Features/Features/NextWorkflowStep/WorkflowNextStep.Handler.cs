using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

internal sealed record WorkflowNextStepCommand(string Code, JsonElement Data);

internal interface IWorkflowNextStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken);
}

internal sealed class WorkflowNextStepHandler(
	ILogger<WorkflowNextStepHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService
	) : IWorkflowNextStepHandler

{
#pragma warning disable MA0051 // Method is too long
	public async Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken)
#pragma warning restore MA0051 // Method is too long
	{
		logger.LogInformation("Advancing workflow {WorkflowCode} from step", request.Code);

		var workflow = await context.Workflows.Include(wf => wf.Steps)
			.ThenInclude(st=> st.Actions)
			.FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
		if (workflow is null)
		{
			return WorkflowErrors.NotFound(request.Code);
		}

		var currentStep = workflow.CurrentStep();
		if (currentStep is null)
		{
			return WorkflowErrors.StepNotFound("SESZH: зачем нужен тип шага? зачем его передавать?");
		}

		var nextStep = workflow.GetNextStep(currentStep.Order);
		if (nextStep is null)
		{
			return WorkflowErrors.NextStepNotFound(request.Code, "SESZH: аналогично");
		}

		//if (!nextStep.Type.Equals(request.StepType, StringComparison.OrdinalIgnoreCase))
		//{
		//	return WorkflowErrors.StepMissMatch(request.Code, nextStep.Type, request.StepType);
		//}

		workflow.CurrentStepType = nextStep.Type; //SESZH: ОЧЕНЬ странная механика.

		//var nextAvailableStep = workflow.GetNextStep(nextStep.Order);
		//const string workflowTypeCode = "ReceiveGoods";
		var response = workflow.ToResponse(request.Data, new WorkflowStepDataSchema
		{
			Version = "1.0",
			DataType = "ReceiveGoods",
			SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
		},
		linkService);
		await context.SaveChangesAsync(cancellationToken);
		//(workflowTypeCode, linkService, nextStep, nextAvailableStep, request.Data);
		//var response = new WorkflowResponse(workflow.Code, workflowTypeCode, workflow.Name, workflow.Description)
		//{
		//	Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
		//	DataSchema = new WorkflowStepDataSchema
		//	{
		//		Version = "1.0",
		//		DataType = "ReceiveGoods",
		//		SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
		//	},
		//CurrentStep = nextAvailableStep.ToCurrentStepResponse(workflowTypeCode, nextStep, )
		/*new WorkflowCurrentStep(nextStep.Type, nextStep.Name, "Step Description")
		{
			Actions = new Actions
			{
				Links = nextStep.Actions
					.Select(action => linkService.Generate(
						action.Endpoint,
						action.RouteParams ?? new Dictionary<string, object> { { "code", workflow.Code } },
						action.Name,
						action.HttpMethod.ToHttpMethod()
					))
					.ToList(),
				NextStep = nextAvailableStep != null 
					? linkService.Generate(
						"WorkflowNextStep",
						new { code = workflow.Code, stepType = nextAvailableStep.Type },
						"Move to Next Step",
						HttpMethod.PATCH
					)
					: null
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
		},*/

		//WorkflowSteps = new List<WorkflowStepShortResponse>
		//{
		//	new WorkflowStepShortResponse("Scan", "Шаг сканирования", 1),
		//	new WorkflowStepShortResponse("Verify", "Шаг проверки", 2),
		//	new WorkflowStepShortResponse("Complete", "Шаг завершения", 3)
		//}
		//};

		return response;
	}


	//This is a sample workflow for demonstration purposes.
	

}
