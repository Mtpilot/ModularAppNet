using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Maintenance.Features.Features.Shared.Responses;
using Modules.Maintenance.Features.Features.Shared.Routes;

namespace Modules.Maintenance.Features.Features.CreateAct;

public sealed record CreateActRequest(
    Guid VehicleId,
    string Description,
    DateTime MaintenanceDate
);

public class CreateActApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.BaseRoute, Handle);
	}
	private static async Task<IResult> Handle(
		[FromBody] CreateActRequest request,
		IValidator<CreateActRequest> validator,
		ICreateActHandler handler,
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
