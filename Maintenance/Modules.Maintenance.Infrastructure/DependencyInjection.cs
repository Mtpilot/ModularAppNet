using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Common.Infrastructure.Database;
using Modules.Common.Infrastructure.Policies;
using Modules.Maintenance.Infrastructure.Database;
using Modules.Maintenance.Infrastructure.Policies;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
	public static IServiceCollection AddMaintenanceInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var postgresConnectionString = configuration.GetConnectionString("Postgres");

		services.AddDbContext<MaintenanceDbContext>(x => x
			.UseNpgsql(postgresConnectionString, npgsqlOptions =>
				npgsqlOptions.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.MaintenanceSchemaName))
			.UseSnakeCaseNamingConvention()
		);

		services.AddScoped<IModuleDatabaseMigrator, MaintenanceDatabaseMigrator>();
		services.AddSingleton<IPolicyFactory, MaintenancePolicyFactory>();

		return services;
	}
}
