using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.Workflows.Infrastructure.Database;

public class WorkflowsDatabaseMigrator : IModuleDatabaseMigrator
{
    public async Task MigrateAsync(
        IServiceScope scope,
        CancellationToken cancellationToken = default)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowsDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}
