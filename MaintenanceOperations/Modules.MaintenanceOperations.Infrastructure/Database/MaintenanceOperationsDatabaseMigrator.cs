using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.MaintenanceOperations.Infrastructure.Database;

public class MaintenanceOperationsDatabaseMigrator : IModuleDatabaseMigrator
{
	public async Task MigrateAsync(
		IServiceScope scope,
		CancellationToken cancellationToken = default)
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<MaintenanceOperationsDbContext>();
		await dbContext.Database.MigrateAsync(cancellationToken);
	}
}
