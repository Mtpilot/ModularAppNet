namespace Modules.Barcoding.Domain.Entities;

public class InvoiceCheckout : InvoiceHeader
{
	public required Guid StepId { get; set; }
	public int TotalItems { get; set; }
	public int AcceptedItems { get; set; }
	public int MissingItems { get; set; }
	public int ExtraItems { get; set; }
}
