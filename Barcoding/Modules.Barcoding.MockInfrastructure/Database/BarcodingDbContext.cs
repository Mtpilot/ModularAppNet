using Microsoft.EntityFrameworkCore;
using Modules.Barcoding.Domain.Entities;


namespace Modules.Barcoding.MockInfrastructure.Database;

public class BarcodingDbContext(DbContextOptions<BarcodingDbContext> options) : DbContext(options)
{
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceCheckout> InvoiceCheckouts { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Invoice>()
            .HasKey(e => e.WorkflowId);
            
        modelBuilder.Entity<Invoice>()
            .OwnsMany(e => e.Lines, line =>
            {
                line.WithOwner();
                line.Property<string>("Id").ValueGeneratedOnAdd();
            });

        modelBuilder.Entity<InvoiceCheckout>()
            .HasKey(e => new { e.WorkflowId, e.StepId });
            
            
        modelBuilder.HasDefaultSchema(DbConsts.MockBarcodingSchemaName);

		modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarcodingDbContext).Assembly);
            
    }
}
