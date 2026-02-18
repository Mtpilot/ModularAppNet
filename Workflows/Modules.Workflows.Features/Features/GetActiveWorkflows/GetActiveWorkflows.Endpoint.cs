
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.GetActiveWorkflows;

public class GetActiveWorkflowsEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetActiveWorkflows, Handle)
			.WithName("GetActiveWorkflows")
			.WithTags("Workflow group")
			.WithSummary("Get all active workflow")
			.WithDescription("Получить весь список активных воркфлоу одного типа.")
			.Produces<List<WorkflowShortInfoResponse>>(StatusCodes.Status200OK);
	}

	private static async Task<IResult> Handle(
		[FromRoute]string workflowTypeCode,
		IGetActiveWorkflowsHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(workflowTypeCode, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
