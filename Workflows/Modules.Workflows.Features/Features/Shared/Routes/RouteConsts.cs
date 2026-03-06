namespace Modules.Workflows.Features.Features.Shared.Routes;


internal static class RouteConsts
{
	internal const string BaseRoute = "/api/workflow";

	internal const string GetWorkflowTypes = $"{BaseRoute}/type";

	internal const string GetWorkflows = $"{BaseRoute}/type/{{workflowTypeCode}}";
	internal const string GetActiveWorkflows = $"{BaseRoute}/type/{{workflowTypeCode}}/active";

	internal const string GetWorkflow = $"{BaseRoute}/{{workflowCode}}";

	internal const string CreateWorkflow = BaseRoute;

	internal const string CancelWorkflow = $"{BaseRoute}/{{workflowCode}}/cancel";

	internal const string CompleteWorkflow = $"{BaseRoute}/{{workflowCode}}/complete";
	internal const string NextStep = $"{BaseRoute}/{{workflowCode}}/next/";
	internal const string PreviousStep = $"{BaseRoute}/{{workflowCode}}/previous/";
	internal const string DropMockDb = $"{BaseRoute}/dropmockdb";
}
