using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Workflows.PublicApi.Responses;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

public sealed class WorkflowNextStepEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.NextStep, Handle)
			.WithName("WorkflowNextStep")
			.WithTags("Workflow group")
			.WithSummary("Transfer workflow to NextStep")
			.WithDescription("Переводим воркфлоу на другой шаг (например из шага \"Сканирования товара\", на шаг \"Проверка накладной\")") //SESZH: из всех мест слетела кодировка ТОЛЬКО здесь и видно это ТОЛЬКО в курсоре, в VS все в порядке
			.Produces<List<WorkflowResponse>>(StatusCodes.Status200OK);
		
	}

	private static async Task<IResult> Handle(
		string workflowCode,
		IWorkflowNextStepHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new WorkflowNextStepCommand(workflowCode);
		var response = await handler.HandleAsync(command, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
