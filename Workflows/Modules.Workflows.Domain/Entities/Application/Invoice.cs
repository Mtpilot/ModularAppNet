
namespace Modules.Workflows.Domain.Entities.Application;

public class Invoice : InvoiceHeader
{
	//SESZH: Это для мокового линкования к шагу
	public required Guid StepId { get; set; }
	//number, date, counterparty, contract, sum, currency, lines
	public ICollection<InvoiceLine> Lines { get; set; }
}

public class InvoiceLine
{
	public required string ProductName { get; set; }
	public required string ConstructorName { get; set; }
	public required string ProductCode { get; set; }
	public required string Barcode { get; set; }
	public required decimal Quantity { get; set; }
	public required string Units { get; set; }
}
