using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Domain.Policies;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.CompleteWorkflow;

public class CompleteWorkflowApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.CompleteWorkflow, Handle)
			.WithName(EndpointConsts.CompleteWorkflow)
			.WithTags("Workflow management")
			.WithSummary("Complete a workflow")
			.WithDescription("Завершение workflow (только если workflow находится на финальном шаге)")
			.RequireAuthorization(WorkflowPolicyConsts.UpdatePolicy)
			.Produces(StatusCodes.Status204NoContent);
	}

	private static async Task<IResult> Handle(
		[FromRoute] string workflowCode,
		ICompleteWorkflowHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(workflowCode, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.NoContent();
	}
}
