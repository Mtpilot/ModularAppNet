using Microsoft.EntityFrameworkCore;
using Modules.Maintenance.Domain.Entities;


namespace Modules.Maintenance.Infrastructure.Database;

public class MaintenanceDbContext(DbContextOptions<MaintenanceDbContext> options) : DbContext(options)
{
	public DbSet<MaintenanceAct> MaintenanceActs { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.HasDefaultSchema(DbConsts.MaintenanceSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(MaintenanceDbContext).Assembly);
	}
}
