using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Maintenance.Features.Features.Shared.Routes;

namespace Modules.Maintenance.Features.Features.AddOperationToAct;

public class AddOperationToActApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.CreateOperationForActRoute, Handle);
	}

	private static async Task<IResult> Handle(
		[FromRoute] Guid actId,
		[FromBody] AddOperationToActRequestBody body,
		IValidator<AddOperationToActRequest> validator,
		IAddOperationToActHandler handler,
		CancellationToken cancellationToken)
	{
		var request = new AddOperationToActRequest(
			ActId: actId,
			VehicleNumberPlate: body.VehicleNumberPlate,
			Description: body.Description,
			OperationDate: body.OperationDate,
			MechanicId: body.MechanicId,
			Cost: body.Cost,
			Tasks: body.Tasks
		);

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

		return Results.Ok();
	}
}

public sealed record AddOperationToActRequestBody(
	string VehicleNumberPlate,
	string Description,
	DateTime OperationDate,
	Guid MechanicId,
	decimal Cost,
	List<string> Tasks);
