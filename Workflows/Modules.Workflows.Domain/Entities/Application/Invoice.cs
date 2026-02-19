using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Workflows.Domain.Entities.Application;

public class Invoice : InvoiceHeader
{
	//SESZH: Это для мокового линкования к шагу
	public required Guid StepId { get; set; }
	public required string Name { get; set; }
	public required DateTime Date { get; set; }
	//number, date, counterparty, contract, sum, currency, lines
	public List<InvoiceLine> Lines { get; set; }
}

public class InvoiceLine
{
	public required string ProductName { get; set; }
	public required string Barcode { get; set; }
	public required decimal Quantity { get; set; }
	public required string Units { get; set; }
}
