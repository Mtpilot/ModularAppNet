using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.MockInfrastructure;

namespace Modules.Workflows.Features.QueryHandlers;

internal sealed class DropMockDb(WorkflowsDbContext context) : IDropMockDb
{
	public async Task DropMockDbAsync(CancellationToken cancellationToken)
	{
		await context.Database.EnsureDeletedAsync(cancellationToken);
		await context.Database.EnsureCreatedAsync(cancellationToken);
		MockDbSeeder.Seed(context);
	}
}