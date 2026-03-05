using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Barcoding.Features.Contracts;

public record InvoiceHeaderDto(string invoiceNumber, string counterparty, string contractNumber, DateTime date) : IBaseWorkflowDataDto;