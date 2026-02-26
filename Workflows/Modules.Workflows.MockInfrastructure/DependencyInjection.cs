using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Common.Infrastructure.Database;
using Modules.Common.Infrastructure.Policies;
using Modules.Workflows.MockInfrastructure;
using Modules.Workflows.MockInfrastructure.Database;
using Microsoft.Data.Sqlite;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
	public static IServiceCollection AddWorkflowsMockInfrastructure(this IServiceCollection services)
	{
		services.AddKeyedSingleton<SqliteConnection>("Workflows", (_, _) =>
		{
			var connection = new SqliteConnection("Filename=:memory:");
			connection.Open();
			return connection;
		});

		services.AddDbContext<WorkflowsDbContext>((sp, opt) =>
		{
			var connection = sp.GetRequiredKeyedService<SqliteConnection>("Workflows");
			opt.UseSqlite(connection);
		});

		services.AddHostedService<MockSeederHostedService>();

		return services;
	}
}
