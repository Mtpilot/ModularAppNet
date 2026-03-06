using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Workflows.Features.Features.PreviousWorkflowStep;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Common.API.Extensions;
using Modules.Workflows.Domain.Policies;


namespace Modules.Workflows.Features.Features.PreviousWorkflowStep;


public sealed class PreviousWorkflowStepApiEndpoint : IApiEndpoint

{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.PreviousStep, Handle)
			.WithName(EndpointConsts.PreviousWorkflowStep)
			.WithTags("Workflow group")
			.WithSummary("Transfer workflow to Previous Step")
			.WithDescription("Переводим воркфлоу на другой шаг (например из шага \"Сканирования товара\", на шаг \"Проверка накладной\") ")
			.RequireAuthorization(WorkflowPolicyConsts.UpdatePolicy)
			.Produces<WorkflowResponse>(StatusCodes.Status200OK);

	}

	private static async Task<IResult> Handle(
		string workflowCode,
		IPreviousWorkflowStepHandler handler,
		CancellationToken cancellationToken)
	{

		var command = new PreviousWorkflowStepCommand(workflowCode);
		var response = await handler.HandleAsync(command, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
