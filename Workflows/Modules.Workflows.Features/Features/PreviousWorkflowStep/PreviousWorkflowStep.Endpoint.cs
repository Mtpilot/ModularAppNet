using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Workflows.Features.Features.PreviousWorkflowStep;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Common.API.Extensions;


namespace Modules.Workflows.Features.Features.PreviousWorkflowStep;

public sealed class PreviousWorkflowStep : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.PreviousStep, Handle)
			.WithName("WorkflowPrevStep")
			.WithTags("Workflow group")
			.WithSummary("Transfer workflow to Previous Step")
			.WithDescription("Переводим воркфлоу на другой шаг (например из шага \"Сканирования товара\", на шаг \"Проверка накладной\") ")
			.Produces<List<WorkflowResponse>>(StatusCodes.Status200OK);

	}

	private static async Task<IResult> Handle(
		string code,
		[FromBody] WorkflowPreviousStepBodyRequest request,
		IValidator<WorkflowPreviousStepBodyRequest> validator,
		IWorkflowPreviousStepHandler handler,
		CancellationToken cancellationToken)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(validationResult.ToDictionary());
		}

		var command = new WorkflowPreviousStepCommand(code, request.Data);
		var response = await handler.HandleAsync(command, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
