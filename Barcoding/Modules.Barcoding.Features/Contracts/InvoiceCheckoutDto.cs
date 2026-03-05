using Modules.Workflows.Domain; //SESZH: выяснить, почему оно не ругается на эту ссылку!!!
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Barcoding.Features.Contracts;

public record InvoiceCheckoutDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date, int totalItems, int acceptedItems, int missingItems, int extraItems) : IBaseStepDataDto;
