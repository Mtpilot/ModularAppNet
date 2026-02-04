using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.MaintenanceOperations.PublicApi;
using Modules.MaintenanceOperations.PublicApi.Contracts;

namespace Modules.Maintenance.Features.Features.AddOperationToAct;

internal interface IAddOperationToActHandler : IHandler
{
    Task<Result<Success>> HandleAsync(
        AddOperationToActRequest request,
        CancellationToken cancellationToken);
}

internal sealed class AddOperationToActHandler(
    IMaintenanceOperationsModuleApi operationsApi
    )
    : IAddOperationToActHandler
{
    public async Task<Result<Success>> HandleAsync(
        AddOperationToActRequest request,
        CancellationToken cancellationToken)
    {
        var operationRequest = new CreateMaintenanceOperationRequest(
            VehicleNumberPlate: request.VehicleNumberPlate,
            Description: request.Description,
            MaintenanceActId: request.ActId,
            OperationDate: request.OperationDate,
            MechanicId: request.MechanicId,
            Cost: request.Cost,
            Tasks: request.Tasks
        );

        return await operationsApi.CreateOperationAsync(operationRequest, cancellationToken);
    }
}
