using Microsoft.EntityFrameworkCore;
using Modules.MaintenanceOperations.Domain.Entities;


namespace Modules.MaintenanceOperations.Infrastructure.Database;

public class MaintenanceOperationsDbContext(DbContextOptions<MaintenanceOperationsDbContext> options) : DbContext(options)
{
	public DbSet<MaintenanceOperation> Operations { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema(DbConsts.MaintenanceOperationsSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaintenanceOperationsDbContext).Assembly);
	}
}
