namespace Modules.Barcoding.Features.Features.Shared.Routes;

internal static class RouteConsts
{
    internal const string BaseRoute = "/api/barcoding";
	internal const string SendScannedBarcodes= $"{BaseRoute}/{{workflowCode}}/{{stepCode}}/sendbarcodes";
}