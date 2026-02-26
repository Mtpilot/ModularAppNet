using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Modules.Workflows.Domain.Serializers;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowStep //<TEntity> //Сканировать, Проверить, Принять
{
	public required Guid Id { get; set; }
	public required Guid WorkflowId { get; set; }
	public required string StepCode { get; set; }
	public required WorkflowStepType Type { get; set; } //Scan, Verify, Accept
	public required string Name { get; set; }
	public required int Order { get; set; }
	public required string Description { get; set; }

	public required List<WorkflowStepAction> Actions { get; set; }
	[NotMapped]
	public IWorkflowDataCollection? Data{get; set;}
}
public enum WorkflowStepType
{
	Scan,
	Verify,
	Accept,
}
