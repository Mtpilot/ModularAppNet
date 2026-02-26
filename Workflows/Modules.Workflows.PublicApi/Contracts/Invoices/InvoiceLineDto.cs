
namespace Modules.Workflows.PublicApi.Contracts;

public record InvoiceLineDto(string productName, string constructorName, string productCode, string barcode, int quantity, string units);