using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;

namespace Modules.Workflows.Features.Features.CompleteWorkflow;

internal interface ICompleteWorkflowHandler : IHandler
{
	Task<Result<Success>> HandleAsync(string code, CancellationToken cancellationToken);
}

internal sealed class CompleteWorkflowHandler(
	ILogger<CompleteWorkflowHandler> logger) : ICompleteWorkflowHandler
{
	public async Task<Result<Success>> HandleAsync(string code, CancellationToken cancellationToken)
	{
		logger.LogInformation("Completing workflow with code '{Code}'", code);

		// TODO: Load workflow from storage (database, cache, etc.)
		var workflow = BuildSampleWorkflow();
		
		if (!workflow.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
		{
			logger.LogDebug("Workflow with code {Code} not found", code);
			return WorkflowErrors.NotFound(code);
		}

		if (!workflow.IsActive)
		{
			logger.LogInformation("Workflow with code {Code} is not active", code);
			return WorkflowErrors.CannotComplete(code, "Workflow is not active");
		}

		if (!workflow.IsFinalStep())
		{
			logger.LogInformation("Workflow with code {Code} is not on final step", code);
			return WorkflowErrors.NotOnFinalStep(code);
		}

		workflow.Complete();

		// TODO: Save workflow to storage
		
		logger.LogInformation("Workflow with code {Code} was completed", code);
		return Result.Success;
	}

	private static Workflow<Dictionary<string, object>> BuildSampleWorkflow()
	{
		// Sample implementation - in real app, this would load from storage
		const string workflowTypeCode = "ReceiveGoods";
		return new Workflow<Dictionary<string, object>>
		{
			Code = "123",
			TypeCode = workflowTypeCode,
			Name = "Приемка по накладной",
			Description = "Приемка по каждой строчки накладной",
			IsActive = true,
			Data = new WorkflowDataCollection(),
			CurrentStepType = "Accept", // Final step for testing
			Steps = new List<WorkflowStep>
			{
				new WorkflowStep
				{
					Type = "Scan",
					Name = "Шаг сканирования",
					Description = "Сканирование товара",
					Order = 1,
					Actions = new List<WorkflowStepAction>()
				},
				new WorkflowStep
				{
					Type = "Verify",
					Name = "Шаг проверки",
					Description = "Проверка количества",
					Order = 2,
					Actions = new List<WorkflowStepAction>()
				},
				new WorkflowStep
				{
					Type = "Accept",
					Name = "Шаг приемки",
					Description = "Подтверждение приемки",
					Order = 3,
					Actions = new List<WorkflowStepAction>()
				}
			}
		};
	}

	private sealed class WorkflowDataCollection : IWorkflowDataCollection<Dictionary<string, object>>
	{
		public string Name { get; set; } = "Data";
		public string Description { get; set; } = "Workflow data";
		public ICollection<Dictionary<string, object>> Collection { get; set; } = new List<Dictionary<string, object>>();
	}
}
