using System.Text.Json;
using Modules.Common.API.Abstractions.Links;

namespace Modules.Workflows.Features.Features.Shared.Responses;

/// <summary>
/// Базовая информация о workflow - общая для всех типов ответов
/// </summary>
public abstract record WorkflowBaseInfoResponse(string Code, string TypeCode, string Name, string Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public string? Data { get; set; }
}

/// <summary>
/// Краткий список workflow (для GET /workflows)
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

/// <summary>
/// Полная информация о workflow с текущим шагом (для GET /workflows/{code} и переходов)
/// </summary>
public sealed record WorkflowResponse(
	string WorkflowCode,
	string WorkflowTypeCode,
	string Name,
	string Description)
	: WorkflowBaseInfoResponse(WorkflowCode, WorkflowTypeCode, Name, Description)
{
	public required WorkflowStepResponse CurrentStep { get; init; }
    public required IList<WorkflowStepShortInfoResponse> WorkflowSteps { get; init; }
}

/// <summary>
/// Информация о шаге в списке (просто справочная информация)
/// </summary>
public record WorkflowStepShortInfoResponse(
    string WorkflowStepCode,
    string WorkflowStepType,
	string Name,
	int Order,
	string Description);

/// <summary>
/// Информация о текущем активном шаге с доступными действиями
/// </summary>
public sealed record WorkflowStepResponse( //Сканировать, Проверить, Принять (Scan, Verify, Accept)
	string WorkflowStepCode,
	string WorkflowStepType,
	string Name,
    int Order,
    string Description): WorkflowStepShortInfoResponse(WorkflowStepCode, WorkflowStepType, Name, Order, Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public required string Data { get; set; } //DefaultWorkflowDataCollection<Invoice/TEntity>
	public required WorkflowActions Actions { get; set; }
}

/// <summary>
/// Доступные действия на текущем шаге
/// </summary>
public sealed record WorkflowActions
{
	/// <summary>
	/// Доступные действия для текущего шага (например, "Increment Qty", "Add Line")
	/// </summary>
	public required IList<Link> StepActions { get; init; }
	
	/// <summary>
	/// Ссылка на переход к следующему шагу (если доступно)
	/// </summary>
	public Link? NextStep { get; set; }
}

/// <summary>
/// JSON Schema информация для валидации данных
/// </summary>
public sealed record WorkflowStepDataSchema
{
	public required string Version { get; set; }
	public required string DataType { get; set; }
	public required string SchemaJson { get; set; }
}
