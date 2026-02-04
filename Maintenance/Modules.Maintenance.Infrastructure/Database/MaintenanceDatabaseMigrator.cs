using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.Infrastructure.Database;

namespace Modules.Maintenance.Infrastructure.Database;

public class MaintenanceDatabaseMigrator : IModuleDatabaseMigrator
{
	public async Task MigrateAsync(
		IServiceScope scope,
		CancellationToken cancellationToken = default)
	{
		var dbContext = scope.ServiceProvider.GetRequiredService<MaintenanceDbContext>();
		await dbContext.Database.MigrateAsync(cancellationToken);
	}
}
