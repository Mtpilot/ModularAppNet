namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Информация о шаге в списке (справочная).
/// </summary>
public record WorkflowStepShortInfoResponse(
	string WorkflowStepCode,
	string WorkflowStepType,
	string Name,
	int Order,
	string Description);
