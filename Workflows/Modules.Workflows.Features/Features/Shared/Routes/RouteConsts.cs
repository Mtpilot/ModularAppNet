namespace Modules.Workflows.Features.Features.Shared.Routes;


internal static class RouteConsts
{
	internal const string BaseRoute = "/api/workflows";

	internal const string GetWorkflowTypes = $"{BaseRoute}/types";

	internal const string GetActive = $"{BaseRoute}/{{workflowType}}/active";

	internal const string GetWorkflow = $"{BaseRoute}/{{code}}";

	internal const string NextStep = $"{BaseRoute}/{{code}}/next/{{stepType}}";
}
