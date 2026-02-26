using Modules.Barcoding.Domain.Entities;
using Modules.Barcoding.MockInfrastructure.Database;
using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Modules.Barcoding.Features.Helpers;

//Временный класс для моков.
public class MockTmpHelper(BarcodingDbContext context) : IMockTmpHelper
{
	public async Task<List<InvoiceHeaderDto>> GetMockInvoiceHeadersFromInMemoryDb(Guid workflowId, CancellationToken cancellationToken)
	{		
		return await context.Invoices.Where(x => x.WorkflowId == workflowId).Select(x=> new InvoiceHeaderDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date)).ToListAsync(cancellationToken);
	}
	public async Task<List<InvoiceDto>> GetMockInvoicesFromInMemoryDb(Guid stepId, CancellationToken cancellationToken)
	{
		return await context.Invoices.AsNoTracking().Include(x=> x.Lines).Where(x => x.StepId == stepId).Select(x=> new InvoiceDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date, x.Lines.Count, x.Lines.Select(y=> new InvoiceLineDto(y.ProductName, y.ConstructorName, y.Barcode, y.Barcode, y.Quantity, y.Units)).ToList())).ToListAsync(cancellationToken);
	}
	public async Task<List<InvoiceCheckoutDto>> GetMockInvoiceCheckoutsFromInMemoryDb(Guid stepId, CancellationToken cancellationToken)
	{
		return await context.InvoiceCheckouts.Where(x => x.StepId == stepId).Select(x=> new InvoiceCheckoutDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date, x.TotalItems, x.AcceptedItems, x.MissingItems, x.ExtraItems)).ToListAsync(cancellationToken);
	}
}
