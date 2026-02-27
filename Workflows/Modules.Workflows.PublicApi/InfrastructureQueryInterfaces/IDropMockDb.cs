


namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IDropMockDb
{
    Task DropMockDbAsync(CancellationToken cancellationToken);
}
