using Modules.Common.API.Abstractions.Links;

namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Краткий элемент списка workflow (например GET /workflows).
/// </summary>
public sealed record WorkflowShortInfoResponse(
	string WorkflowCode,
	string WorkflowTypeCode,
	string Name,
	string Description)
	: WorkflowBaseInfoResponse(WorkflowCode, WorkflowTypeCode, Name, Description)
{
	public required string CurrentStepType { get; set; }
	public required string CurrentStepName { get; set; }
	public required IList<Link> Links { get; init; }
}
