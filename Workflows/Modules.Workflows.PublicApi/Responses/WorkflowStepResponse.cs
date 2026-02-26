using Modules.Common.API.Abstractions.Links;

namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Текущий активный шаг с данными и доступными действиями.
/// </summary>
public sealed record WorkflowStepResponse(
	string WorkflowStepCode,
	string WorkflowStepType,
	string Name,
	int Order,
	string Description)
	: WorkflowStepShortInfoResponse(WorkflowStepCode, WorkflowStepType, Name, Order, Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public required string Data { get; set; }
	public required WorkflowActions Actions { get; set; }
}
