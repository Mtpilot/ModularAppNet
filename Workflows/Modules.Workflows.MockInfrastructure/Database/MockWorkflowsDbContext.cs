using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.MockInfrastructure.Database;

public class WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : DbContext(options)
{
	public DbSet<Workflow> Workflows { get; set; }
	public DbSet<WorkflowStep> WorkflowSteps { get; set; }
	public DbSet<WorkflowType> WorkflowTypes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<WorkflowStep>()
			.Ignore(x => x.Data);

		modelBuilder.Entity<WorkflowStepAction>()
			.Ignore(e => e.RouteParams);

		modelBuilder.HasDefaultSchema(DbConsts.MockWorkflowsSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowsDbContext).Assembly);
	}
}
