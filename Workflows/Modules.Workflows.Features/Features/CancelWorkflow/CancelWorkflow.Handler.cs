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
	Task<Result<Success>> HandleAsync(string workflowCode, CancellationToken cancellationToken);
}

internal sealed class CancelWorkflowHandler(
	WorkflowsDbContext context,
	ILogger<CancelWorkflowHandler> logger) : ICancelWorkflowHandler
{
	public async Task<Result<Success>> HandleAsync(string workflowCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Cancelling workflow with code '{Code}'", workflowCode);

        // TODO: Load workflow from storage (database, cache, etc.)
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
			return WorkflowErrors.CannotCancel(workflowCode, "Workflow is not active");
		}

		workflow.Cancel();

		await context.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Workflow with code {Code} was cancelled", workflowCode);
		return Result.Success;
	}
}
