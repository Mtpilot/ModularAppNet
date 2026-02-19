using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;

namespace Modules.Workflows.MockInfrastructure.Database;

public class WorkflowsDbContext(DbContextOptions<WorkflowsDbContext> options) : DbContext(options)
{
	public DbSet<Workflow> Workflows { get; set; } //SESZH: временно добавлю наследника
	public DbSet<WorkflowStep> WorkflowSteps { get; set; }
	public DbSet<WorkflowStepAction> WorkflowStepActions { get; set; }
	public DbSet<DataItemInventory> WorkflowDataItemInventory { get; set; }
	public DbSet<WorkflowType> WorkflowTypes { get; set; }
	public DbSet<InvoiceHeader> InvoiceHeaders { get; set; } //TODO: Нет хранимого типа InvoiceHeader, только Invoice. InvoiceHeader получается проекций из Invoice при чтении из хранилища.
															// зачитываются Invoice к процессу, делается проекция в InvoiceHeader,вставляет в Data процесса,
															// а сам Invoice вставляется в CurrentStep (если шаг scan) 

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
			.Ignore(e => e.Data)
			;

		modelBuilder.Entity<WorkflowStepAction>()
			.Ignore(e => e.RouteParams);

		modelBuilder.Entity<InvoiceHeader>()
			.HasKey(e => e.WorkflowId);

		modelBuilder.HasDefaultSchema(DbConsts.MockWorkflowsSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkflowsDbContext).Assembly);
	}
}
