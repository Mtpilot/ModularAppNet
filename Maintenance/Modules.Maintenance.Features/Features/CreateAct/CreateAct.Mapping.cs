using Modules.Maintenance.Domain.Entities;
using Modules.Maintenance.Features.Features.Shared.Responses;

namespace Modules.Maintenance.Features.Features.CreateAct;

internal static class CreateActMappingExtensions
{
    public static ActResponse MapToResponse(this MaintenanceAct act)
    {
        return new ActResponse(
            act.Id,
            act.VehicleId,
            act.Description,
            act.MaintenanceDate
        );
    }
}
