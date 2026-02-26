using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Workflows.PublicApi.Requests;
using Modules.Workflows.PublicApi.Responses;

namespace Modules.Workflows.Features.Features.CreateWorkflow;

public class CreateWorkflowEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.CreateWorkflow, Handle)
			.WithName("CreateWorkflow")
			.WithTags("Workflow management")
			.WithSummary("Create a new workflow")
			.WithDescription("Создание нового workflow с начальным шагом")
			.Produces<WorkflowResponse>(StatusCodes.Status200OK);
	}

	private static async Task<IResult> Handle(
		[FromBody] CreateWorkflowRequest request,
		IValidator<CreateWorkflowRequest> validator,
		ICreateWorkflowHandler handler,
		CancellationToken cancellationToken)
	{
		var validationResult = await validator.ValidateAsync(request, cancellationToken);
		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(validationResult.ToDictionary());
		}

		var response = await handler.HandleAsync(request, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
