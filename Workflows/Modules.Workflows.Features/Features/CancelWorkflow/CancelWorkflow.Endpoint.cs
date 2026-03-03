using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Domain.Policies;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.CancelWorkflow;

public class CancelWorkflowEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.CancelWorkflow, Handle)
			.WithName("CancelWorkflow")
			.WithTags("Workflow management")
			.WithSummary("Cancel an active workflow")
			.WithDescription("Отмена активного workflow")
			.RequireAuthorization(WorkflowPolicyConsts.UpdatePolicy)
			.Produces(StatusCodes.Status204NoContent);
	}

	private static async Task<IResult> Handle(
		[FromRoute] string workflowCode,
		ICancelWorkflowHandler handler,
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
