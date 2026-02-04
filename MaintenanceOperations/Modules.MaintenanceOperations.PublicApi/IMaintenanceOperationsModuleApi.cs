using Modules.MaintenanceOperations.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.MaintenanceOperations.PublicApi;

public interface IMaintenanceOperationsModuleApi
{
	Task<Result<Success>> CreateOperationAsync(CreateMaintenanceOperationRequest request, CancellationToken cancellationToken);
}
