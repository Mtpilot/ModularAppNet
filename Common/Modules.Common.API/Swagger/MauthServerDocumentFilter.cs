using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Modules.Common.API.Swagger;

/// <summary>
/// Sets the OpenAPI server URL so that Swagger UI sends requests to the correct path behind nginx (/external/mauth).
/// Without this, "Try it out" would call /api/... and nginx would return 404.
/// </summary>
public sealed class MauthServerDocumentFilter : IDocumentFilter
{
	public void Apply(OpenApiDocument document, DocumentFilterContext context)
	{
		document.Servers = new List<OpenApiServer>
		{
			new OpenApiServer { Url = "/external/mauth" }
		};
	}
}
