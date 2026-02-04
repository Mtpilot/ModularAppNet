using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Common.Infrastructure.Database;
using Modules.Common.Infrastructure.Policies;
using Modules.MaintenanceOperations.Infrastructure.Database;
using Modules.MaintenanceOperations.Infrastructure.Policies;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
	public static IServiceCollection AddMaintenanceOperationsInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var postgresConnectionString = configuration.GetConnectionString("Postgres");

		services.AddDbContext<MaintenanceOperationsDbContext>(x => x
			.UseNpgsql(postgresConnectionString, npgsqlOptions =>
				npgsqlOptions.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.MaintenanceOperationsSchemaName))
			.UseSnakeCaseNamingConvention()
		);

		services.AddScoped<IModuleDatabaseMigrator, MaintenanceOperationsDatabaseMigrator>();
		services.AddSingleton<IPolicyFactory, MaintenanceOperationsPolicyFactory>();

		return services;
	}
}
