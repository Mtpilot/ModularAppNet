
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Modules.Common.API.Abstractions;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Workflows.Domain.Policies;

namespace Modules.Workflows.Features.Features.DropMockDB;

public class DropMockDbApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.DropMockDb, Handle)
			.WithName(EndpointConsts.DropMockDb)
			.WithTags("Workflow management")
			.WithSummary("Drop mock database")
			.WithDescription("Drop mock database")
			.RequireAuthorization(WorkflowPolicyConsts.DeletePolicy)
			.Produces(StatusCodes.Status204NoContent);
	}

	private static async Task<IResult> Handle(
		IDropMockDbHandler handler,
		CancellationToken cancellationToken)
	{
		await handler.HandleAsync(cancellationToken);
		return Results.NoContent();
	}
}
