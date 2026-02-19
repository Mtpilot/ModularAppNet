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
	internal const string NextStep = $"{BaseRoute}/{{workflowCode}}/next/";//SESZH: я подумал-подумал и не понял, зачем нам тип шага, когда мы идем к следующему в конкретном воркфлоу, если внутри этого самого воркфлоу есть инфа и о нынешнем и о следующем и о всех вообще{{stepType}}";
	internal const string PreviousStep = $"{BaseRoute}/{{workflowCode}}/back/";//{{stepType}}";

	internal const string SendScannedBarcodes= $"{BaseRoute}/{{stepCode}}/sendbarcodes";
}
