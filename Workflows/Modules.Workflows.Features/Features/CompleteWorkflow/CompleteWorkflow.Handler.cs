using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.Features.Features.CompleteWorkflow;

internal interface ICompleteWorkflowHandler : IHandler
{
	Task<Result<Success>> HandleAsync(string workflowCode, CancellationToken cancellationToken);
}

internal sealed class CompleteWorkflowHandler(
	WorkflowsDbContext context,
	ILogger<CompleteWorkflowHandler> logger) : ICompleteWorkflowHandler
{
	public async Task<Result<Success>> HandleAsync(string workflowCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Completing workflow with code '{Code}'", workflowCode);

		var workflow = await context.Workflows.Include(wf => wf.Steps)
            .ThenInclude(st => st.Actions)
            .FirstOrDefaultAsync(x => x.Code == workflowCode, cancellationToken);

        if (!workflow.Code.Equals(workflowCode, StringComparison.OrdinalIgnoreCase))
		{
			logger.LogDebug("Workflow with code {Code} not found", workflowCode);
			return WorkflowErrors.NotFound(workflowCode);
		}

		if (!workflow.IsActive)
		{
			logger.LogInformation("Workflow with code {Code} is not active", workflowCode);
			return WorkflowErrors.CannotComplete(workflowCode, "Workflow is not active");
		}

		if (!workflow.IsFinalStep())
		{
			logger.LogInformation("Workflow with code {Code} is not on final step", workflowCode);
			return WorkflowErrors.NotOnFinalStep(workflowCode);
		}

		workflow.Complete();

		await context.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Workflow with code {Code} was completed", workflowCode);
		return Result.Success;
	}
}
