using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Workflows.Domain.Policies;
using Modules.Workflows.PublicApi.Responses;

namespace Modules.Workflows.Features.Features.GetWorkflow;

public class GetWorkflowApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetWorkflow, Handle)
			.WithName(EndpointConsts.GetWorkflow)
			.WithTags("Workflow group")
			.WithSummary("Get workflow by code")
			.WithDescription("Получить workflow по коду")
			.RequireAuthorization(WorkflowPolicyConsts.ReadPolicy)
			.Produces<WorkflowResponse>(StatusCodes.Status200OK);
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
