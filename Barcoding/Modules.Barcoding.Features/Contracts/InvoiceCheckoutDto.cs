using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Barcoding.Features.Contracts;

public record InvoiceCheckoutDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date, int totalItems, int acceptedItems, int missingItems, int extraItems) : IBaseStepDataDto;
