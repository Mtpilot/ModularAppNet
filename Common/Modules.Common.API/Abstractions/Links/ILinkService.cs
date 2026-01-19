namespace Modules.Common.API.Abstractions.Links;

public interface ILinkService
{
	Link Generate(string endPointName, object? routeValues, string rel, HttpMethod method);
}
