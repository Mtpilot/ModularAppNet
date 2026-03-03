using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Modules.Common.Infrastructure.Database;
using Modules.Common.Infrastructure.Policies;
using Modules.Workflows.Infrastructure.Database;
using Modules.Workflows.Infrastructure.Policies;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddWorkflowsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var postgresConnectionString = configuration.GetConnectionString("Postgres");

        services.AddDbContext<WorkflowsDbContext>(x => x
            .UseNpgsql(postgresConnectionString, npgsqlOptions => 
                npgsqlOptions.MigrationsHistoryTable(DbConsts.MigrationHistoryTableName, DbConsts.WorkflowsSchemaName))
            .UseSnakeCaseNamingConvention()
        );
        
        services.AddScoped<IModuleDatabaseMigrator, WorkflowsDatabaseMigrator>();
        services.AddSingleton<IPolicyFactory, WorkflowsPolicyFactory>();

        return services;
    }
}
