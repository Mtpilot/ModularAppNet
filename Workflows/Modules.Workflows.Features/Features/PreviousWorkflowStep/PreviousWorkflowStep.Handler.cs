using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Workflows.Features.Features.PreviousWorkflowStep;

internal sealed record PreviousWorkflowStepCommand(string Code);

internal interface IPreviousWorkflowStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(PreviousWorkflowStepCommand request, CancellationToken cancellationToken);
}

internal sealed class PreviousWorkflowStepHandler(
	ILogger<PreviousWorkflowStepHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService,
    IInMemoryDbHelper mockTmpHelper
	) : IPreviousWorkflowStepHandler

{
	public async Task<Result<WorkflowResponse>> HandleAsync(PreviousWorkflowStepCommand request, CancellationToken cancellationToken)
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
        var workflowData = new WorkflowDataCollection<IBaseWorkflowDataDto> { Name = workflow.Name, Description = workflow.Description, Collection = await mockTmpHelper.GetWorkflowDataFromInMemoryDb(workflow.Id, cancellationToken) };
		var stepData = new WorkflowDataCollection<IBaseStepDataDto> { Name = previousStep.Name, Description = previousStep.Description, Collection = await mockTmpHelper.GetStepDataFromInMemoryDb(previousStep.Id, previousStep.Type, cancellationToken) };
		return workflow.ConvertWorkflowToResponse(linkService, workflowData, stepData);	
    }
}
