using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.Domain.Serializers;

public static class DataItemSerializer
{
	public static readonly JsonSerializerOptions DataItemSeializerOptions = new()
	{
		WriteIndented = false,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		TypeInfoResolver = new DefaultJsonTypeInfoResolver
		{
			Modifiers = { AddPolymorphism }
		}
	};

	private static void AddPolymorphism(JsonTypeInfo ti)
	{
		if (ti.Type == typeof(IDataItem))
		{
			ti.PolymorphismOptions = new JsonPolymorphismOptions
			{
				TypeDiscriminatorPropertyName = "$type",
				DerivedTypes =
			{
				new JsonDerivedType(typeof(DataItemInventory), "inventory"),
				//new JsonDerivedType(typeof(DataItemOther), "other"),
                // все наследники IDataItem
            }
			};
		}
	}
	private static readonly JsonSerializerOptions Options = new()
	{
		PropertyNameCaseInsensitive = true,
		WriteIndented = false,
	};

	public static string Serialize(List<Dictionary<string, object>>? data)
	{
		if (data is null || data.Count == 0)
		{
			return "[]";
		}
		return JsonSerializer.Serialize(data, Options);
	}

	public static List<Dictionary<string, object>> Deserialize(string? dataJson)
	{
		if (string.IsNullOrWhiteSpace(dataJson))
		{
			return [];
		}
		var list = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(dataJson, Options);
		return list ?? [];
	}
}
