
namespace Modules.Workflows.PublicApi.Contracts;

public record InvoiceDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date, int totalItems, List<InvoiceLineDto> lines);
