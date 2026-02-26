using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Infrastructure.Database;

public class WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : DbContext(options)
{
    public DbSet<Workflow> Workflows { get; set; }
	public DbSet<WorkflowStep> WorkflowSteps { get; set; }
	public DbSet<WorkflowStepAction> WorkflowStepActions { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(DbConsts.WorkflowsSchemaName);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowsDbContext).Assembly);
    }
}
