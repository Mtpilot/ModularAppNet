using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Microsoft.EntityFrameworkCore;

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
			return WorkflowErrors.StepNotFound("SESZH: зачем нужен тип шага? зачем его передавать?");
		}

		var previousStep = workflow.GetPreviousStep(currentStep.Order);
		if (previousStep is null)
		{
			return WorkflowErrors.PreviousStepNotFound(request.Code, "SESZH: аналогично");
		}

		//workflow.CurrentStepType = previousStep.Type; //SESZH: все еще ОЧЕНЬ странная механика.
		workflow.SetStepNumber(previousStep.Order);

		var response = workflow.ToResponse(new WorkflowStepDataSchema
		{
			Version = "1.0",
			DataType = "ReceiveGoods",
			SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
		},
		linkService);

		await context.SaveChangesAsync(cancellationToken);
		return response;
	}
}
