using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.Domain.Handlers;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

namespace Modules.Workflows.Features.Features.DropMockDB;



internal interface IDropMockDbHandler : IHandler
{
	Task HandleAsync(CancellationToken cancellationToken);
}


internal sealed class DropMockDbHandler(
	ILogger<DropMockDbHandler> logger,
	IEnumerable<IDropMockDb> dropMockDbImplementations) : IDropMockDbHandler
{
	public async Task HandleAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Dropping mock database(s)");
		foreach (var dropMockDb in dropMockDbImplementations)
		{
			await dropMockDb.DropMockDbAsync(cancellationToken);
		}
	}
}
