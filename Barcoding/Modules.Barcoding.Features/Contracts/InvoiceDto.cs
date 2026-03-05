using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Barcoding.Features.Contracts;

public record InvoiceDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date, int totalItems, List<InvoiceLineDto> lines) : IBaseStepDataDto;