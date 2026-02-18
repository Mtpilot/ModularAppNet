using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.GetWorkflow;

public class GetWorkflowEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetWorkflow, Handle)
			.WithName("GetWorkflow")
			.WithTags("Workflow group");
	}

	private static async Task<IResult> Handle(
		[FromRoute] string workflowCode,
		IGetWorkflowHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(workflowCode, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
