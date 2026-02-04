//using Bogus;
using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Modules.Common.Domain.Events;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.MaintenanceOperations.Domain.Entities;
//using Modules.Stocks.PublicApi;
//using Modules.Stocks.PublicApi.Contracts;

using Modules.MaintenanceOperations.Features.Features.Shared.Responses;
//using Modules.Shipments.Features.Features.Shared.Errors;
//using Modules.Shipments.Features.Features.Shared.Responses;
using Modules.MaintenanceOperations.Infrastructure.Database;
using Modules.MaintenanceOperations.PublicApi.Contracts;

namespace Modules.MaintenanceOperations.Features.Features.CreateOperation;

internal interface ICreateOperationHandler : IHandler
{
	Task<Result<OperationResponse>> HandleAsync(CreateMaintenanceOperationRequest request, CancellationToken cancellationToken);
}


internal sealed class CreateOperationHandler(
	MaintenanceOperationsDbContext context
	)
	: ICreateOperationHandler
{
	public async Task<Result<OperationResponse>> HandleAsync(
		CreateMaintenanceOperationRequest request,
		CancellationToken cancellationToken)
	{

		var operation = new MaintenanceOperation
		{
			Id = Guid.NewGuid(),
			VehicleNumberPlate = request.VehicleNumberPlate,
			Description = request.Description,
			OperationDate = request.OperationDate,
			MechanicId = request.MechanicId,
			MaintenanceActId = request.MaintenanceActId,
			Cost = request.Cost,
			Tasks = request.Tasks,
		};

		await context.Operations.AddAsync(operation, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);

		return operation.MapToResponse();

		//return shipment.MapToResponse();
	}

	//SESZH: пока не понимаю, что оно все делает, будет пусто
	//private static CheckStockRequest CreateCheckStockRequest(CreateShipmentRequest request)
	//{
	//	return new CheckStockRequest(
	//		request.Items
	//			.Select(x => new ProductStock(x.Product, x.Quantity))
	//			.ToList()
	//	);
	//}
}
