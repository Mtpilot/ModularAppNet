using Modules.Workflows.PublicApi.Contracts;


namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IInMemoryDbHelper //SESZH: переименовал методы на более абстрактные, оставаясь только на уровне воркфлоу
	// Убрал темп и мок, потому что смысл этого интерфейса и наследников - доставать данные из бд в памяти
{
    Task<List<IBaseWorkflowDataDto>> GetWorkflowDataFromInMemoryDb(Guid workflowId, CancellationToken cancellationToken);
	Task<List<IBaseStepDataDto>> GetStepDataFromInMemoryDb(Guid stepId, string stepType, CancellationToken cancellationToken); //SESZH: пока лучшее, что я смог придумать - отдавать тип шага для принятия решения о. Я подумаю еще.
}
