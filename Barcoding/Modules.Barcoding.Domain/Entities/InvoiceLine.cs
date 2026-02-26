
namespace Modules.Barcoding.Domain.Entities;

public class InvoiceLine
{
	public required string ProductName { get; set; }
	public required string ConstructorName { get; set; }
	public required string ProductCode { get; set; }
	public required string Barcode { get; set; }
	public required int Quantity { get; set; }
	public required string Units { get; set; }
}
