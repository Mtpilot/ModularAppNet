namespace Modules.Workflows.Features.Features.Shared.Routes;


internal static class RouteConsts
{
	internal const string BaseRoute = "/api/workflows";

	internal const string GetWorkflowTypes = $"{BaseRoute}/types";

	internal const string GetActive = $"{BaseRoute}/{{workflowTypeCode}}/active";

	internal const string GetWorkflow = $"{BaseRoute}/{{code}}";

	internal const string NextStep = $"{BaseRoute}/{{code}}/next/";//SESZH: я подумал-подумал и не понял, зачем нам тип шага, когда мы идем к следующему в конкретном воркфлоу, если внутри этого самого воркфлоу есть инфа и о нынешнем и о следующем и о всех вообще{{stepType}}";
	internal const string PreviousStep = $"{BaseRoute}/{{code}}/back/";//{{stepType}}";

	internal const string SendScannedBarcodes= $"{BaseRoute}/{{code}}/data/sendbarcodes";
}
