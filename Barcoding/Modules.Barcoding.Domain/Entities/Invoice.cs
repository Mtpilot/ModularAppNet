
namespace Modules.Barcoding.Domain.Entities;

public class Invoice : InvoiceHeader
{
	//SESZH: Это для мокового линкования к шагу
	public required Guid StepId { get; set; }
	//number, date, counterparty, contract, sum, currency, lines
	public ICollection<InvoiceLine> Lines { get; set; }
}
