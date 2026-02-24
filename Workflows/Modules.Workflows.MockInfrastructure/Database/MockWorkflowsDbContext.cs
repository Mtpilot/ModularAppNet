using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;

namespace Modules.Workflows.MockInfrastructure.Database;

public class WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : DbContext(options)
{
	public DbSet<Workflow> Workflows { get; set; }
	public DbSet<WorkflowStep> WorkflowSteps { get; set; }
	public DbSet<WorkflowStepAction> WorkflowStepActions { get; set; }
	public DbSet<DataItemInventory> WorkflowDataItemInventory { get; set; }
	public DbSet<WorkflowType> WorkflowTypes { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<InvoiceCheckout> InvoiceCheckouts { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<WorkflowStep>()
			.Ignore(x => x.Data);



		modelBuilder.Entity<WorkflowStepAction>()
			.Ignore(e => e.RouteParams);

		modelBuilder.Entity<Invoice>()
			.HasKey(e => e.WorkflowId);

		modelBuilder.Entity<Invoice>()
			.OwnsMany(e => e.Lines, line =>
			{
				line.WithOwner();
				line.Property<string>("Id").ValueGeneratedOnAdd();
			});

		modelBuilder.Entity<InvoiceCheckout>()
			.HasKey(e => e.WorkflowId);

		modelBuilder.HasDefaultSchema(DbConsts.MockWorkflowsSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowsDbContext).Assembly);
	}
}
