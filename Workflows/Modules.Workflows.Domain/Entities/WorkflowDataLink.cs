using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowDataLink
{
	[Key]
	public required Guid DataId { get; set; }
	public required string DataType { get; set; } //SESZH: это то, в какой таблице искать данные. 
}
