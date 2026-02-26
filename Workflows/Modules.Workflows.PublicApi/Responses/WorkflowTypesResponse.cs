namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Элемент списка типов workflow (GET /api/workflows/types).
/// </summary>
public sealed record WorkflowTypesResponse(string WorkflowTypeCode, string Name);
