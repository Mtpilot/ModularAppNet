using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Barcoding.MockInfrastructure.Database;

namespace Modules.Barcoding.MockInfrastructure;

internal sealed class MockSeederHostedService(IServiceProvider serviceProvider) : IHostedService
{
	public async Task StartAsync(CancellationToken ct)
	{
	    await using var scope = serviceProvider.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<BarcodingDbContext>();

		await db.Database.EnsureCreatedAsync(ct);
		var tableNames = await db.Database
	.SqlQueryRaw<string>("SELECT name FROM sqlite_master WHERE type='table'")
	.ToListAsync(ct);
		MockDbSeeder.Seed(db);
	}

	public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
