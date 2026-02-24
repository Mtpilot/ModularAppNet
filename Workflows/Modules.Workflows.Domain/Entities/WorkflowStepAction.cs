using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowStepAction
{
	public required Guid Id { get; set; }
	public required Guid StepId { get; set; }
	public required string Type { get; set; }              // "IncrementQty", "AddLine" и т.д.
	public required string Name { get; set; }             // "Increment Qty"
	public required string Description { get; set; }

	// Новые поля для генерации ссылок
	public required string Endpoint { get; set; }         // "WorkflowItem", "WorkflowLineItem"
	public required string HttpMethod { get; set; }   // POST, PUT, PATCH
	
	public string RouteParamsJson { get; set; } = string.Empty;

	[NotMapped]
	public required Dictionary<string, object> RouteParams
	{
		get => JsonSerializer.Deserialize<Dictionary<string, object>>(RouteParamsJson ?? "{}")!;
		set => RouteParamsJson = JsonSerializer.Serialize(value);
	}
}
