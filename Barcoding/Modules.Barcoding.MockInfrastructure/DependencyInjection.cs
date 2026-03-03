using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Barcoding.MockInfrastructure;
using Modules.Barcoding.MockInfrastructure.Database;
using Microsoft.Data.Sqlite;
using Modules.Barcoding.MockInfrastructure.Policies;
using Modules.Common.Infrastructure.Policies;


namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddBarcodingMockInfrastructure(this IServiceCollection services)
    {
        services.AddKeyedSingleton<SqliteConnection>("Barcoding", (_, _) =>
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        });

        services.AddDbContext<BarcodingDbContext>((sp, opt) =>
        {
            var connection = sp.GetRequiredKeyedService<SqliteConnection>("Barcoding");
            opt.UseSqlite(connection);
        });

        services.AddHostedService<MockSeederHostedService>();
        services.AddSingleton<IPolicyFactory, BarcodingPolicyFactory>();
        return services;
    }
}
