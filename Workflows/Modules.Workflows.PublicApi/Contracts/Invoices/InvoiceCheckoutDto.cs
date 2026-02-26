
namespace Modules.Workflows.PublicApi.Contracts;

public record InvoiceCheckoutDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date, int totalItems, int acceptedItems, int missingItems, int extraItems);
