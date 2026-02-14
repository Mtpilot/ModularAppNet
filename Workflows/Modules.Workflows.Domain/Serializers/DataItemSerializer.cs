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
}
