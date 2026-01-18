using Microsoft.AspNetCore.Routing;
using Modules.Common.API.Abstractions.Links;

namespace Modules.Common.API.Services;

internal sealed class LinkService(LinkGenerator linkGenerator) : ILinkService
{
	public Link Generate(string endPointName, object? routeValues, string rel, string method)
	{
		var href = linkGenerator.GetPathByName(endPointName, routeValues) ?? string.Empty;
		return new Link(href, rel, method);
	}
}
