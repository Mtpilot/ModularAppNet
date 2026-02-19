using System.Collections.Generic;
using System.Text.Json;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Domain.Serializers;

/// <summary>
/// Сериализация IWorkflowDataCollection{TEntity} в строку для хранения в БД и обратный parse для использования в Features.
/// </summary>
public static class DataCollectionSerializer
{
	public static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = false,
		PropertyNameCaseInsensitive = true,
	};

	/// <summary>
	/// Сериализует данные шага workflow в строку для сохранения в БД.
	/// </summary>
	/// <typeparam name="TEntity">Тип элемента коллекции (например, <see cref="Entities.Application.Invoice"/>).</typeparam>
	public static string Serialize<TEntity>(IWorkflowDataCollection<TEntity>? data)
	{
		if (data is null)
		{
			return "{}";
		}
		return JsonSerializer.Serialize(data, Options);
	}

	/// <summary>
	/// Десериализует строку из БД в IWorkflowDataCollection{TEntity}. В Features можно использовать для parse после чтения DataJson.
	/// </summary>
	/// <typeparam name="TEntity">Тип элемента коллекции (например, <see cref="Entities.Application.Invoice"/>).</typeparam>
	public static IWorkflowDataCollection<TEntity>? Deserialize<TEntity>(string? dataJson)
	{
		if (string.IsNullOrWhiteSpace(dataJson))
		{
			return null;
		}
		var value = JsonSerializer.Deserialize<DefaultWorkflowDataCollection<TEntity>>(dataJson, Options);
		return value;
	}
}
