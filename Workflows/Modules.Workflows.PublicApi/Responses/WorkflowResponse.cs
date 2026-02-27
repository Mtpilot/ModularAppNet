using Modules.Common.API.Abstractions.Links;

namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Полный ответ по workflow с текущим шагом (GET /workflows/{code}, переходы, SendScannedBarcodes).
/// </summary>
public sealed record WorkflowResponse(
	string WorkflowCode,
	string WorkflowTypeCode,
	string Name,
	string Description, 
	bool IsActive)
	: WorkflowBaseInfoResponse(WorkflowCode, WorkflowTypeCode, Name, Description, IsActive)
{
	public required WorkflowStepResponse CurrentStep { get; init; }
	public required IList<WorkflowStepShortInfoResponse> WorkflowSteps { get; init; }
}
