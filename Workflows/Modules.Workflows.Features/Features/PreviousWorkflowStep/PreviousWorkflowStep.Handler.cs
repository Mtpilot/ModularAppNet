using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Infrastructure.Helpers;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.Features.Features.PreviousWorkflowStep;

internal sealed record WorkflowPreviousStepCommand(string Code);

internal interface IWorkflowPreviousStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(WorkflowPreviousStepCommand request, CancellationToken cancellationToken);
}

internal sealed class WorkflowPreviousStepHandler(
	ILogger<WorkflowPreviousStepHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService
	) : IWorkflowPreviousStepHandler

{
#pragma warning disable MA0051 // Method is too long
	public async Task<Result<WorkflowResponse>> HandleAsync(WorkflowPreviousStepCommand request, CancellationToken cancellationToken)
#pragma warning restore MA0051 // Method is too long
	{
		logger.LogInformation("Advancing workflow {WorkflowCode} from step", request.Code);

		var workflow = await context.Workflows
            .Include(wf => wf.Type)
			.Include(wf => wf.Steps)
			.ThenInclude(st => st.Actions)
			.FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);
		if (workflow is null)
		{
			return WorkflowErrors.NotFound(request.Code);
		}

		var currentStep = workflow.CurrentStep();
		if (currentStep is null)
		{
			return WorkflowErrors.StepNotFound($"for {workflow.Code}");
		}

		var previousStep = workflow.GetPreviousStep(currentStep.Order);
		if (previousStep is null)
		{
			return WorkflowErrors.PreviousStepNotFound(request.Code, currentStep.StepCode);
		}

		workflow.SetStepNumber(previousStep.Order);
		await context.SaveChangesAsync(cancellationToken);
        var tmpWorkflowData = await MockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(context, workflow.Id, cancellationToken);
        switch (workflow.CurrentStep().Type) //SESZH: надо срочно доделывать сигнатуры и начинать очистку от этого всего, потом завязну, оно все нарастает
        {
            case WorkflowStepType.Scan:
                {
                    var tmpStepData = await MockTmpHelper.GetMockInvoicesFromInMemoryDb(context, workflow.CurrentStep().Id, cancellationToken);
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
            case WorkflowStepType.Verify:
                {
                    var tmpStepData = await MockTmpHelper.GetMockInvoiceCheckoutsFromInMemoryDb(context, workflow.CurrentStep().Id, cancellationToken);
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
            default:
                throw new NotSupportedException("Current step type not supported");
        }
    }
}
