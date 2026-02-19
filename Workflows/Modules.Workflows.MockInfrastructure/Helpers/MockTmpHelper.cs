using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;

using Modules.Workflows.MockInfrastructure.Database;
using Microsoft.EntityFrameworkCore;
namespace Modules.Workflows.Infrastructure.Helpers;

//Временный класс для моков
public static class MockTmpHelper
{
	public static async Task<DefaultWorkflowDataCollection<InvoiceHeader>> GetMockInvoiceHeadersFromInMemoryDb(WorkflowsDbContext context, Guid workflowId, CancellationToken cancellationToken)
	{		
		return new DefaultWorkflowDataCollection<InvoiceHeader> { Name = "InvoiceHeaders", Description = "Collection of invoice headers", Collection = await context.InvoiceHeaders.Where(x => x.WorkflowId == workflowId).ToListAsync(cancellationToken) };
	}
}
