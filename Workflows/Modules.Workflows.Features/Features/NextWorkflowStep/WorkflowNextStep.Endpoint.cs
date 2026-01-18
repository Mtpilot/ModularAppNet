using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

public sealed class WorkflowNextStepEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.NextStep, Handle)
			.WithName("WorkflowNextStep");
	}

	private static async Task<IResult> Handle(
		string code,
		string type,
		WorkflowNextStepBodyRequest request,
		IWorkflowNextStepHandler handler,
		CancellationToken cancellationToken)
	{
		var command = new WorkflowNextStepCommand(code, type, request.Data);
		var response = await handler.HandleAsync(command, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
