using System.Text.Json;

namespace Modules.Workflows.PublicApi.Requests;

/// <summary>
/// Запрос на создание нового workflow (POST /api/workflows).
/// </summary>
public sealed record CreateWorkflowRequest(
	string TypeCode,
	string TypeName,
	string Name,
	string Description,
	JsonElement Data);
