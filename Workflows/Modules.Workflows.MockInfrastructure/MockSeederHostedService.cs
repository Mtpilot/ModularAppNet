using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.MockInfrastructure;

internal sealed class MockSeederHostedService(IServiceProvider serviceProvider) : IHostedService
{
	public async Task StartAsync(CancellationToken ct)
	{
	    await using var scope = serviceProvider.CreateAsyncScope();
		var db = scope.ServiceProvider.GetRequiredService<WorkflowsDbContext>();

		await db.Database.EnsureCreatedAsync(ct);

		MockDbSeeder.Seed(db);
	}

	public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}
