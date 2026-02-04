using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Maintenance.Infrastructure.Database;
using Modules.Maintenance.Features.Features.Shared.Responses;
using Modules.Maintenance.Domain.Entities;

namespace Modules.Maintenance.Features.Features.CreateAct;

internal interface ICreateActHandler : IHandler
{
    Task<Result<ActResponse>> HandleAsync(
        CreateActRequest request,
        CancellationToken cancellationToken);
}

internal sealed class CreateActHandler(
	MaintenanceDbContext context
	)
	: ICreateActHandler
{
    public async Task<Result<ActResponse>> HandleAsync(
        CreateActRequest request,
        CancellationToken cancellationToken)
    {

        var act = new MaintenanceAct
        {
            Id = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            MaintenanceDate = request.MaintenanceDate,
            Description = request.Description,
		};

        await context.MaintenanceActs.AddAsync(act, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return act.MapToResponse();
    }
}
