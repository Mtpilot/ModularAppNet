using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Responses;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

internal sealed record WorkflowNextStepCommand(string Code, string Type, Dictionary<string, object> Data);

internal interface IWorkflowNextStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken);
}

internal sealed class WorkflowNextStepHandler(ILogger<WorkflowNextStepHandler> logger, ILinkService linkService) : IWorkflowNextStepHandler

{
	public Task<Result<WorkflowResponse>> HandleAsync(WorkflowNextStepCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Advancing workflow {WorkflowCode} from step {StepType}", request.Code, request.Type);

		var workflow = BuildSampleWorkflow(); //TODO: поднимаем вокрфлоу по коду для перевода на следующий шаг
		if (!workflow.Code.Equals(request.Code, StringComparison.OrdinalIgnoreCase))
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.NotFound(request.Code));
		}

		var currentStep = workflow.CurrentStep();
		if (currentStep is null)
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.StepNotFound(request.Type));
		}

		var nextStep = workflow.GetNextStep(currentStep.Order);
		if (nextStep is null)
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.NextStepNotFound(request.Code, request.Type));
		}

		if (!nextStep.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase))
		{
			return Task.FromResult<Result<WorkflowResponse>>(WorkflowErrors.StepMissMatch(request.Code, nextStep.Type, request.Type));
		}

		var nextAvailableStep = workflow.GetNextStep(nextStep.Order);
		var response = new WorkflowResponse(workflow.Code, workflow.Name, workflow.Description)
		{
			CurrentStep = new WorkflowCurrentStep(nextStep.Type, nextStep.Name, "Step Description")
			{
				Actions = new Actions
				{
					Links = new List<Link>
					{
						new Link("/api/item","Increment Qty","POST"),
						new Link("/api/", "Add Line", "PUT")
					},
					NextStep = nextAvailableStep != null ? linkService.Generate("WorkflowNextStep", new { code = workflow.Code, nextAvailableStep.Type }, "Move to Next Step", "PATCH") : null
				}
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

		return new Workflow<Dictionary<string, object>>
		{
			Code = "123",
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
}
