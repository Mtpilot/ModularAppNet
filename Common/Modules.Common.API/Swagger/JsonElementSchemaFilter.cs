using System.Text.Json;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Modules.Common.API.Swagger;

/// <summary>
/// Schema filter that converts JsonElement to object type in Swagger/OpenAPI schema.
/// This allows JsonElement fields to be displayed as JSON objects instead of strings.
/// </summary>
public sealed class JsonElementSchemaFilter : ISchemaFilter
{
	public void Apply(OpenApiSchema schema, SchemaFilterContext context)
	{
		if (context.Type == typeof(JsonElement))
		{
			// Replace JsonElement with object type in Swagger schema
			schema.Type = "object";
			schema.Properties = null;
			schema.AdditionalPropertiesAllowed = true;
			schema.Description = "Arbitrary JSON object. Can contain objects, arrays, and nested structures.";
		}
	}
}
