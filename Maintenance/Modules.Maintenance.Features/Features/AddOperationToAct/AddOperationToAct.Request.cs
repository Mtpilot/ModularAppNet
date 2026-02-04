namespace Modules.Maintenance.Features.Features.AddOperationToAct;

public sealed record AddOperationToActRequest(
    Guid ActId,
    string VehicleNumberPlate,
    string Description,
    DateTime OperationDate,
    Guid MechanicId,
    decimal Cost,
    List<string> Tasks);
