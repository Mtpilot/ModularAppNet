namespace Modules.Workflows.Features.Features.Shared.Routes;


internal static class RouteConsts
{
	internal const string BaseRoute = "/api/workflows";

	internal const string GetActive = $"{BaseRoute}/active";

	internal const string NextStep = $"{BaseRoute}/{{code}}/next/{{type}}";
}
