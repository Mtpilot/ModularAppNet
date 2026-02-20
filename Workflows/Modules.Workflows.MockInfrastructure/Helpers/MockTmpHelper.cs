using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;

using Modules.Workflows.MockInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace Modules.Workflows.Infrastructure.Helpers;

//Временный класс для моков
public static class MockTmpHelper
{
	public static async Task<DefaultWorkflowDataCollection<InvoiceHeader>> GetMockInvoiceHeadersFromInMemoryDb(WorkflowsDbContext context, Guid workflowId, CancellationToken cancellationToken)
	{		
		return new DefaultWorkflowDataCollection<InvoiceHeader> { Name = "InvoiceHeaders", Description = "Collection of invoice headers", Collection = await context.Invoices.Where(x => x.WorkflowId == workflowId).Select(x=> new InvoiceHeader { WorkflowId = x.WorkflowId, InvoiceNumber = x.InvoiceNumber, ContractNumber = x.ContractNumber, Counterparty = x.Counterparty, Date = x.Date }).ToListAsync(cancellationToken) };
	}
	public static async Task<DefaultWorkflowDataCollection<Invoice>> GetMockInvoicesFromInMemoryDb(WorkflowsDbContext context, Guid stepId, CancellationToken cancellationToken)
	{
		return new DefaultWorkflowDataCollection<Invoice> { Name = "Invoices", Description = "Collection of invoices", Collection = await context.Invoices.AsNoTracking().Include(x=> x.Lines).Where(x => x.StepId == stepId).Select(x=> new Invoice { StepId = x.StepId, WorkflowId = x.WorkflowId, InvoiceNumber = x.InvoiceNumber, ContractNumber = x.ContractNumber, Counterparty = x.Counterparty, Date = x.Date, Lines = x.Lines }).ToListAsync(cancellationToken) };
	}
	public static async Task<DefaultWorkflowDataCollection<InvoiceCheckout>> GetMockInvoiceCheckoutsFromInMemoryDb(WorkflowsDbContext context, Guid stepId, CancellationToken cancellationToken)
	{
		return new DefaultWorkflowDataCollection<InvoiceCheckout> { Name = "InvoiceCheckouts", Description = "Collection of invoice checkouts", Collection = await context.InvoiceCheckouts.Where(x => x.StepId == stepId).Select(x=> new InvoiceCheckout { StepId = x.StepId, WorkflowId = x.WorkflowId, InvoiceNumber = x.InvoiceNumber, ContractNumber = x.ContractNumber, Counterparty = x.Counterparty, Date = x.Date,}).ToListAsync(cancellationToken) };
	}
}
