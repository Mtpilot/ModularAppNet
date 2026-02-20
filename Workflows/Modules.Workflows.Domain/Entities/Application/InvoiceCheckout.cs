using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities.Application;

public class InvoiceCheckout : InvoiceHeader
{
	public required Guid StepId { get; set; }
	public int TotalItems { get; set; }
	public int AcceptedItems { get; set; }
	public int MissingItems { get; set; }
	public int ExtraItems { get; set; }
}
