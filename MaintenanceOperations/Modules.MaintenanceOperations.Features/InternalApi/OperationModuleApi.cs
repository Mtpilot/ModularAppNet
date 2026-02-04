using Modules.Common.Domain.Results;
using Modules.MaintenanceOperations.Features.Features.CreateOperation;
using Modules.MaintenanceOperations.PublicApi;
using Modules.MaintenanceOperations.PublicApi.Contracts;

namespace Modules.MaintenanceOperations.Features.InternalApi;

internal sealed class OperationModuleApi(
	ICreateOperationHandler createOperationHandler) : IMaintenanceOperationsModuleApi
{
	public async Task<Result<Success>> CreateOperationAsync(
		CreateMaintenanceOperationRequest request,
		CancellationToken cancellationToken)
	{
		var result = await createOperationHandler.HandleAsync(request, cancellationToken);
		if (result.IsError)
		{
			return result.Errors;
		}

		return Result.Success;
	}
}
