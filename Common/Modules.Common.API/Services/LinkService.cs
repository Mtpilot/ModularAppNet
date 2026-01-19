using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions.Links;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Common.API.Services;

internal sealed class LinkService(LinkGenerator linkGenerator) : ILinkService
{
	public Link Generate(string endPointName, object? routeValues, string rel, HttpMethod method)
	{
		var href = linkGenerator.GetPathByName(endPointName, routeValues) ?? string.Empty;
		return new Link(href, rel, method);
	}
}
