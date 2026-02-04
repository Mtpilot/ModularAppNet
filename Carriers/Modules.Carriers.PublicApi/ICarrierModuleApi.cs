using Modules.Carriers.PublicApi.Contracts;
using Modules.Common.Domain.Results;

namespace Modules.Carriers.PublicApi;

public interface ICarrierModuleApi
{
	//SESZH: почему шимпент создается в кэрриере, а вызывает его как раз шипмент?
    Task<Result<Success>> CreateShipmentAsync(CreateCarrierShipmentRequest request, CancellationToken cancellationToken);
}
