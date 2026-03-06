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
	public required string Type { get; set; } //Scan, Verify, Accept //SESZH: убрал тип и вернул строковое значение, так как могут быть и другие типы у других модулей
	public required string Name { get; set; }
	public required int Order { get; set; }
	public required string Description { get; set; }

	public required List<WorkflowStepAction> Actions { get; set; }
	[NotMapped]
	public IWorkflowDataCollection? Data { get; set; }
}
