using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.MockInfrastructure.Database;
using System;
using System.Collections.Generic;

namespace Modules.Workflows.Features.Features.CancelWorkflow;

internal interface ICancelWorkflowHandler : IHandler
{
	Task<Result<Success>> HandleAsync(string code, CancellationToken cancellationToken);
}

internal sealed class CancelWorkflowHandler(
	WorkflowsDbContext context,
	ILogger<CancelWorkflowHandler> logger) : ICancelWorkflowHandler
{
	public async Task<Result<Success>> HandleAsync(string code, CancellationToken cancellationToken)
	{
		logger.LogInformation("Cancelling workflow with code '{Code}'", code);

        // TODO: Load workflow from storage (database, cache, etc.)
        var workflow = await context.Workflows.Include(wf => wf.Steps)
            .ThenInclude(st => st.Actions)
            .FirstOrDefaultAsync(x => x.Code == code, cancellationToken);

        if (!workflow.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
		{
			logger.LogDebug("Workflow with code {Code} not found", code);
			return WorkflowErrors.NotFound(code);
		}

		if (!workflow.IsActive)
		{
			logger.LogInformation("Workflow with code {Code} is not active", code);
			return WorkflowErrors.CannotCancel(code, "Workflow is not active");
		}

		workflow.Cancel();

		// TODO: Save workflow to storage
		
		logger.LogInformation("Workflow with code {Code} was cancelled", code);
		return Result.Success;
	}

	//private static Workflow<Dictionary<string, object>> BuildSampleWorkflow()
	//{
	//	// Sample implementation - in real app, this would load from storage
	//	const string workflowTypeCode = "ReceiveGoods";
	//
    //    var id = Guid.NewGuid();
    //    return new Workflow<Dictionary<string, object>>
	//	{
    //        Id = id,
    //        WorkflowDataItems = new List<WorkflowDataItem>(),
    //        Code = "123",
	//		TypeCode = workflowTypeCode,
	//		Name = "Приемка по накладной",
	//		Description = "Приемка по каждой строчки накладной",
	//		IsActive = true,
	//		Data = new WorkflowDataCollection(),
	//		CurrentStepType = "Scan",
	//		CurrentStepNumber = 1,
	//		Steps = new List<WorkflowStep>
	//		{
	//			new WorkflowStep
	//			{
    //                Id = Guid.NewGuid(),
    //                WorkflowId= id,
    //                Type = "Scan",
	//				Name = "Шаг сканирования",
	//				Description = "Сканирование товара",
	//				Order = 1,
	//				Actions = new List<WorkflowStepAction>()
	//			},
	//			new WorkflowStep
	//			{
    //                Id = Guid.NewGuid(),
    //                WorkflowId= id,
    //                Type = "Verify",
	//				Name = "Шаг проверки",
	//				Description = "Проверка количества",
	//				Order = 2,
	//				Actions = new List<WorkflowStepAction>()
	//			},
	//			new WorkflowStep
	//			{
    //                Id = Guid.NewGuid(),
    //                WorkflowId= id,
    //                Type = "Accept",
	//				Name = "Шаг приемки",
	//				Description = "Подтверждение приемки",
	//				Order = 3,
	//				Actions = new List<WorkflowStepAction>()
	//			}
	//		}
	//	};
	//}

	private sealed class WorkflowDataCollection : IWorkflowDataCollection<Dictionary<string, object>>
	{
		public string Name { get; set; } = "Data";
		public string Description { get; set; } = "Workflow data";
		public ICollection<Dictionary<string, object>> Collection { get; set; } = new List<Dictionary<string, object>>();
	}
}
