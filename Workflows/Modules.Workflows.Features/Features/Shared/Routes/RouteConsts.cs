namespace Modules.Workflows.Features.Features.Shared.Routes;


internal static class RouteConsts
{
	internal const string BaseRoute = "/api/workflows";

	internal const string GetWorkflowTypes = $"{BaseRoute}/types";

	internal const string GetWorkflows = $"{BaseRoute}/types/{{workflowTypeCode}}";
	internal const string GetActiveWorkflows = $"{BaseRoute}/types/{{workflowTypeCode}}/active";

	internal const string GetWorkflow = $"{BaseRoute}/{{workflowCode}}";

	internal const string CreateWorkflow = BaseRoute;

	internal const string CancelWorkflow = $"{BaseRoute}/{{workflowCode}}/cancel";

	internal const string CompleteWorkflow = $"{BaseRoute}/{{workflowCode}}/complete";
	internal const string NextStep = $"{BaseRoute}/{{workflowCode}}/next/";
	internal const string PreviousStep = $"{BaseRoute}/{{workflowCode}}/back/";

	internal const string SendScannedBarcodes= $"{BaseRoute}/{{workflowCode}}/{{stepCode}}/sendbarcodes";
}
