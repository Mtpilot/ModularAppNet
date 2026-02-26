namespace Modules.Workflows.PublicApi.Requests;

/// <summary>
/// Элемент тела запроса отправки отсканированных штрих-кодов (SendScannedBarcodes).
/// </summary>
public sealed record ScannedBarcodePayload
{
	public required string Barcode { get; set; }
	public required int Quantity { get; set; }
}
