
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;

using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.GetActiveWorkflows;

public class GetActiveWorkflowsEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetActive, Handle);
	}

	private static async Task<IResult> Handle(		
		IGetActiveWorkflowsHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
