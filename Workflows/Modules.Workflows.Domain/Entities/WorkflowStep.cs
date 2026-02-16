using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowStep //Сканировать, Проверить, Принять
{
	public required Guid Id { get; set; }
	public required Guid WorkflowId { get; set; }
	public required string Type { get; set; } //Scan, Verify, Accept
	public required string Name { get; set; }
	public required int Order { get; set; }
	public required string Description { get; set; }

	public required List<WorkflowStepAction> Actions { get; set; }
}
