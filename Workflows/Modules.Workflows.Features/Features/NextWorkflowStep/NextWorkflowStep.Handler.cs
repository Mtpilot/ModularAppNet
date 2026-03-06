using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

internal sealed record NextWorkflowStepCommand(string Code);

internal interface INextWorkflowStepHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(NextWorkflowStepCommand request, CancellationToken cancellationToken);
}

internal sealed class NextWorkflowStepHandler(
	ILogger<NextWorkflowStepHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService,
    IInMemoryDbHelper mockTmpHelper
	) : INextWorkflowStepHandler

{
	public async Task<Result<WorkflowResponse>> HandleAsync(NextWorkflowStepCommand request, CancellationToken cancellationToken)
	{
		logger.LogInformation("Advancing workflow {WorkflowCode} from step", request.Code);

		var workflow = await context.Workflows
			.Include(wf=> wf.Type)
			.Include(wf => wf.Steps)
			.ThenInclude(st=> st.Actions)
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

		var nextStep = workflow.GetNextStep();
		if (nextStep is null)
		{
			return WorkflowErrors.NextStepNotFound(request.Code, currentStep.StepCode);
		}

		workflow.SetStepNumber(nextStep.Order);

		await context.SaveChangesAsync();
		var workflowData = new WorkflowDataCollection<IBaseWorkflowDataDto> { Name = workflow.Name, Description = workflow.Description, Collection = await mockTmpHelper.GetWorkflowDataFromInMemoryDb(workflow.Id, cancellationToken) };
		var stepData = new WorkflowDataCollection<IBaseStepDataDto> { Name = nextStep.Name, Description = nextStep.Description, Collection = await mockTmpHelper.GetStepDataFromInMemoryDb(nextStep.Id, nextStep.Type, cancellationToken)};
		return workflow.ConvertWorkflowToResponse(linkService, workflowData, stepData);
    }
}
