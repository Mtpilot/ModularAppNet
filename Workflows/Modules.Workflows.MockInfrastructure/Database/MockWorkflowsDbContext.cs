using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.MockInfrastructure.Database;

public class WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : DbContext(options)
{
	public DbSet<Workflow<IWorkflowDataCollectionEntity>> Workflows { get; set; }
	public DbSet<WorkflowStep> WorkflowSteps { get; set; }
	public DbSet<WorkflowStepAction> WorkflowStepActions { get; set; }
	public DbSet<DataItemInventory> WorkflowDataItemInventory { get; set; }
	public DbSet<WorkflowType> WorkflowTypes { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// WorkflowDataItem удалён; данные теперь в WorkflowStep.Data для шагов типа Scan
		//modelBuilder.Entity<Workflow<IDataItem>>()
		//.HasMany(w => w.WorkflowDataItems)
		//.WithOne(d => d.Workflow)
		//.HasForeignKey(d => d.WorkflowId)
		//.OnDelete(DeleteBehavior.Cascade);
		//modelBuilder.Entity<WorkflowDataItem>().HasKey(x => x.Id);
		//modelBuilder.Entity<WorkflowDataItem>().Property(x => x.Discriminator).HasMaxLength(50).IsRequired();

		//modelBuilder.Entity<Workflow<IDataItem>>()
		//	.Property(x => x.DataGuids)
		//	.HasColumnType("uuid[]");

		modelBuilder.Entity<WorkflowStep>()
			.Ignore(x => x.Data);

		//modelBuilder.Entity<Workflow<IDataItem>>()
		//	.Property(x=> x.Data)
		//	.HasConversion(
		//v => JsonSerializer.Serialize(v, JsonOptions),
		//v => JsonSerializer.Deserialize<IWorkflowDataItemsCollection<IDataItem>>(v, JsonOptions)!);

		modelBuilder.Entity<WorkflowStep>()
			.Property(e => e.DataJson)
			.HasColumnType("text");

		modelBuilder.Entity<WorkflowStepAction>()
			.Ignore(e => e.RouteParams);

		modelBuilder.HasDefaultSchema(DbConsts.MockWorkflowsSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowsDbContext).Assembly);
	}
}
