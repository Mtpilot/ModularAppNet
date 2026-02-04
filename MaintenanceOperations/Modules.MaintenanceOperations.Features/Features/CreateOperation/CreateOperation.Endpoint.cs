using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.MaintenanceOperations.Features.Features.Shared.Routes;
using Modules.MaintenanceOperations.PublicApi.Contracts;


namespace Modules.MaintenanceOperations.Features.Features.CreateOperation;

//public sealed record CreateOperationRequest(
//		string VehicleNumberPlate,
//	string Description,
//	Guid MaintenanceActId,
//	DateTime OperationDate,
//	Guid MechanicId,
//	decimal Cost,
//	List<string> Tasks);

public class CreateOperationApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPost(RouteConsts.BaseRoute, Handle);
	}
	private static async Task<IResult> Handle(
		[FromBody] CreateMaintenanceOperationRequest request,
		IValidator<CreateMaintenanceOperationRequest> validator,
		ICreateOperationHandler handler,
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
