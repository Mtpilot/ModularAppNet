using Modules.Barcoding.MockInfrastructure.Database;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Barcoding.MockInfrastructure;

namespace Modules.Barcoding.Features.QueryHandlers;

internal sealed class DropMockDb(BarcodingDbContext context) : IDropMockDb
{
	public async Task DropMockDbAsync(CancellationToken cancellationToken)
	{
		await context.Database.EnsureDeletedAsync(cancellationToken);
		await context.Database.EnsureCreatedAsync(cancellationToken);
		MockDbSeeder.Seed(context);
	}
}
