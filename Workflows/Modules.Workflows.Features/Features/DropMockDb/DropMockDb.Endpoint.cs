
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.DropMockDB;

public class DropMockDbEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.DropMockDB, Handle)
			.WithName("DropMockDB")
			.WithTags("Workflow management")
			.WithSummary("Drop mock database")
			.WithDescription("Drop mock database")
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
