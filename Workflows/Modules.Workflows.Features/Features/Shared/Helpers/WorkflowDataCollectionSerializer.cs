using System.Text.Encodings.Web;
using System.Text.Json;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Features.Features.Shared.Helpers;

internal static class WorkflowDataCollectionSerializer
{
	private static readonly JsonSerializerOptions Options = new()
	{
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		WriteIndented = false,
		PropertyNameCaseInsensitive = true,
	};

	/// <summary>
	/// Сериализует WorkflowDataCollection в JSON. Поддержка полиморфных DTO (InvoiceHeaderDto, InvoiceDto и т.д.) и кириллицы.
	/// </summary>
	public static string Serialize<T>(WorkflowDataCollection<T> data) where T : class
	{
		var toSerialize = new { data.Name, data.Description, Collection = data.Collection.Select(x => (object)x!).ToList() };
		return JsonSerializer.Serialize(toSerialize, Options);
	}
}
