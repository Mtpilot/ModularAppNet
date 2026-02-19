using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Workflows.Domain.Entities.Application;

public class InvoiceHeader
{
	//SESZH: это для мокового линкования к воркфлоу
	public required Guid WorkflowId { get; set; }
	public required string InvoiceId { get; set; }
	public required string Counterparty { get; set; } 
	public required string Contract { get; set; }
}


