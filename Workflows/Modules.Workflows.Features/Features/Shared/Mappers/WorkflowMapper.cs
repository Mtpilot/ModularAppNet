using System.Text.Json;
using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Features.Features.Shared.Responses;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.Shared.Mappers;

internal static class WorkflowMapper
{
	/// <summary>
	/// Domain Workflow → API Response
	/// </summary>
	internal static WorkflowResponse ToResponse(
		this Workflow<IWorkflowDataCollectionEntity> workflow,
		JsonElement data, 
		WorkflowStepDataSchema dataSchema,
		ILinkService linkService)
	{
		//TODO: Mapper не подходящий класс для вычисления логики перехода на следующий шаг. Но пока можно оставить
		var currentStep = workflow.CurrentStep();
		var nextAvailableStep = workflow.GetNextStep(currentStep.Order);

		return new WorkflowResponse(
			workflow.Code,
			workflow.Type.Code,
			workflow.Name,
			workflow.Description)
		{
			DataSchema = dataSchema,
			Data = data, //TODO: ты передаешь сюда данные Шага, это видно по Мокам (где ты заполняешь)
			CurrentStep = currentStep.ToCurrentStepResponse(
				workflow.Code,
				nextAvailableStep,
				dataSchema,
				data,
				linkService),
			WorkflowSteps = workflow.Steps
				.Select(s => new WorkflowStepShortInfoResponse(s.StepCode, s.Type, s.Name, s.Order,  s.Description))
				.ToList()
		};
	}

	/// <summary>
	/// Domain WorkflowStep → API Response для краткого списка
	/// </summary>
	internal static WorkflowShortInfoResponse ToShortResponse(
		this Workflow<IWorkflowDataCollectionEntity> workflow,
		JsonElement data,
		WorkflowStepDataSchema dataSchema,
		ILinkService linkService)
	{
		var currentStep = workflow.CurrentStep();

		return new WorkflowShortInfoResponse(
			workflow.Code,
			workflow.Type.Code,
			workflow.Name,
			workflow.Description)
		{
			CurrentStepType = currentStep.Type,
			CurrentStepName = currentStep.Name,
			DataSchema = dataSchema,
			Data = data,
			Links = new List<Link>
			{
				linkService.Generate(
					"GetWorkflow",
					new { code = workflow.Code },
					"Self",
					HttpMethod.GET)
			}
		};
	}

	/// <summary>
	/// Domain WorkflowStep → API CurrentStep с действиями
	/// </summary>
	internal static WorkflowStepResponse ToCurrentStepResponse(
		this WorkflowStep step,
		string workflowCode,
		WorkflowStep? nextStep,
		WorkflowStepDataSchema dataSchema,
		JsonElement data,
		ILinkService linkService)
	{
		return new WorkflowStepResponse(step.StepCode, step.Type, step.Name, step.Order ,step.Description)
		{
			DataSchema = dataSchema,
			Data = data,
			Actions = new WorkflowActions
			{
				StepActions = step.Actions
					.Select(action => linkService.Generate(
						action.Endpoint,
						new { code = workflowCode },
						//action.RouteParams ?? new Dictionary<string, object> { { "code", workflowCode } },
						action.Name,
						action.HttpMethod.ToHttpMethod()))
					.ToList(),
				NextStep = nextStep != null
					? linkService.Generate(
						"WorkflowNextStep",
						new { code = workflowCode },
						"Move to Next Step",
						HttpMethod.PATCH)
					: null
			}
		};
	}
	/// <summary>
	/// Маппим строку HTTP метода в enum
	/// </summary>
	/// <exception cref="InvalidOperationException"></exception>
	internal static HttpMethod ToHttpMethod(this string method) => method.ToUpper() switch
	{
		"GET" => HttpMethod.GET,
		"POST" => HttpMethod.POST,
		"PUT" => HttpMethod.PUT,
		"PATCH" => HttpMethod.PATCH,
		"DELETE" => HttpMethod.DELETE,
		"HEAD" => HttpMethod.HEAD,
		"OPTIONS" => HttpMethod.OPTIONS,
		_ => throw new InvalidOperationException($"Unknown HTTP method: {method}")
	};
}
