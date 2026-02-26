using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IMockTmpHelper
{
    Task<List<InvoiceHeaderDto>> GetMockInvoiceHeadersFromInMemoryDb(Guid workflowId, CancellationToken cancellationToken);
	Task<List<InvoiceDto>> GetMockInvoicesFromInMemoryDb(Guid stepId, CancellationToken cancellationToken);
	Task<List<InvoiceCheckoutDto>> GetMockInvoiceCheckoutsFromInMemoryDb(Guid stepId, CancellationToken cancellationToken);
}
