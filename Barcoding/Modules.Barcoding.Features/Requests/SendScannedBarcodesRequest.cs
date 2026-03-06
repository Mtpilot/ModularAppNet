
namespace Modules.Barcoding.Features.Requests;

public sealed record SendScannedBarcodesRequest(string WorkflowCode, string StepCode, List<ScannedBarcodesPayload> ScannedBarcodes);
