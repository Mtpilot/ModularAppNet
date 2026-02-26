

namespace Modules.Workflows.PublicApi.Contracts;

public record InvoiceHeaderDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date);
