namespace Modules.Barcoding.Features.Requests;

/// <summary>
/// Элемент тела запроса отправки отсканированных штрих-кодов (SendScannedBarcodes).
/// </summary>
public sealed record ScannedBarcodesPayload
{
	public required string Barcode { get; set; }
	public required int Quantity { get; set; }
}
