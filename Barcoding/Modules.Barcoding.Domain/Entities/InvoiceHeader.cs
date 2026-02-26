namespace Modules.Barcoding.Domain.Entities;

public class InvoiceHeader
{
	//SESZH: это для мокового линкования к воркфлоу
	public required Guid WorkflowId { get; set; }
	public required string InvoiceNumber { get; set; }
	public required string Counterparty { get; set; } 
	public required string ContractNumber { get; set; }
	public required DateTime Date { get; set; }
}


