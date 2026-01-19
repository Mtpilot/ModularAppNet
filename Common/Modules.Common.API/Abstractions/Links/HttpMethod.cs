using System.Text.Json.Serialization;

namespace Modules.Common.API.Abstractions.Links;

/// <summary>
/// HTTP methods supported for API links.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum HttpMethod
{
	GET,
	POST,
	PUT,
	PATCH,
	DELETE,
	HEAD,
	OPTIONS
}
