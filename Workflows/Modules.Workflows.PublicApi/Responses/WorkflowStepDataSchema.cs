namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// JSON Schema информация для валидации данных шага.
/// </summary>
public sealed record WorkflowStepDataSchema
{
	public required string Version { get; set; }
	public required string DataType { get; set; }
	public required string SchemaJson { get; set; }
}
