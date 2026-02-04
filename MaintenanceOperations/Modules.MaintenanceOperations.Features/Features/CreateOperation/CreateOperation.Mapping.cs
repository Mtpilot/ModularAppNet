using Modules.MaintenanceOperations.Domain.Entities;
using Modules.MaintenanceOperations.Features.Features.Shared.Responses;

namespace Modules.MaintenanceOperations.Features.Features.CreateOperation;

internal static class CreateOperationMappingExtensions
{
	public static OperationResponse MapToResponse(this MaintenanceOperation operation)
	{
		return new OperationResponse(
			operation.Id,
			operation.VehicleNumberPlate,
			operation.Description,
			operation.MaintenanceActId,
			operation.OperationDate,
			operation.MechanicId,
			operation.Cost,
			operation.Tasks
		);
	}
}
