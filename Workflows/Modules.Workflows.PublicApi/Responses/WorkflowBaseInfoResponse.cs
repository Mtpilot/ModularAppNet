namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Базовая информация о workflow для всех типов ответов.
/// </summary>
public abstract record WorkflowBaseInfoResponse(string Code, string TypeCode, string Name, string Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public string? Data { get; set; }
}
