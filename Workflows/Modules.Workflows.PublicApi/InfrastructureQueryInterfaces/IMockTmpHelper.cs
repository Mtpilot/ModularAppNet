using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.Domain.Entities;

namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IMockTmpHelper //SESZH: переименовал методы на более абстрактные, оставаясь только на уровне воркфлоу
{
    Task<List<IBaseWorkflowDataDto>> GetMockWorkflowDataFromInMemoryDb(Guid workflowId, CancellationToken cancellationToken);
	Task<List<IBaseStepDataDto>> GetMockStepDataFromInMemoryDb(Guid stepId, string stepType, CancellationToken cancellationToken); //SESZH: пока лучшее, что я смог придумать - отдавать тип шага для дальнейшего использования, я подумаю еще.
}
