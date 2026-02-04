namespace Modules.Maintenance.Features.Features.Shared.Responses;

public sealed record ActResponse(
    Guid Id,
    Guid VehicleId,
    string Description,
    DateTime MaintenanceDate);
