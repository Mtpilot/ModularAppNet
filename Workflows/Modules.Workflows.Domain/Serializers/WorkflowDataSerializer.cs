using System.Text.Json;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Domain.Serializers;

/// <summary>
/// Сериализация IWorkflowData{TEntity} в строку для хранения в БД и обратный parse для использования в Features.
/// </summary>
public static class WorkflowDataSerializer
{
	public static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = false,
		PropertyNameCaseInsensitive = true,
	};

	/// <summary>
	/// Сериализует данные workflow в строку для сохранения в БД.
	/// </summary>
	public static string Serialize<TEntity>(IWorkflowData<TEntity>? data)
	{
		if (data is null)
			return "{}";
		return JsonSerializer.Serialize(data, Options);
	}

	/// <summary>
	/// Десериализует строку из БД в IWorkflowData{TEntity}. В Features можно использовать для parse после чтения DataJson.
	/// </summary>
	public static IWorkflowData<TEntity>? Deserialize<TEntity>(string? dataJson)
	{
		if (string.IsNullOrWhiteSpace(dataJson))
			return null;
		var value = JsonSerializer.Deserialize<DefaultWorkflowData<TEntity>>(dataJson, Options);
		return value;
	}
}
