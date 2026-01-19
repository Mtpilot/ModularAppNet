using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

public sealed class WorkflowNextStepEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.NextStep, Handle)
			.WithName("WorkflowNextStep")
			.WithTags("Workflow group")
			.WithSummary("Transfer workflow to NextStep")
			.WithDescription("Переводим воркфлоу на другой шаг (например из шага \"Сканирования товара\", на шаг \"Проверка накладной\") ")
			.Produces<List<WorkflowResponse>>(StatusCodes.Status200OK);
		
	}

	private static async Task<IResult> Handle(
		string code,
		string stepType,
		[FromBody] WorkflowNextStepBodyRequest request,
		IValidator<WorkflowNextStepBodyRequest> validator,
		IWorkflowNextStepHandler handler,
		CancellationToken cancellationToken)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(validationResult.ToDictionary());
		}

		var command = new WorkflowNextStepCommand(code, stepType, request.Data);
		var response = await handler.HandleAsync(command, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
