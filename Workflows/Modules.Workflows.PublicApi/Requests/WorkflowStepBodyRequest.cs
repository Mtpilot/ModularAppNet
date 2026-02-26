using System.Text.Json;

namespace Modules.Workflows.PublicApi.Requests;

/// <summary>
/// Тело запроса перехода на следующий шаг (PATCH next).
/// </summary>
public sealed record WorkflowNextStepBodyRequest
{
	public required JsonElement Data { get; set; }
}

/// <summary>
/// Тело запроса перехода на предыдущий шаг (PATCH back).
/// </summary>
public sealed record WorkflowPreviousStepBodyRequest
{
	public required JsonElement Data { get; set; }
}
