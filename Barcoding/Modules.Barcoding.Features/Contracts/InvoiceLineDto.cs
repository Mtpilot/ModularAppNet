using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Barcoding.Features.Contracts;

public record InvoiceLineDto(string productName, string constructorName, string productCode, string barcode, int quantity, string units) : IBaseDataItemDto;    