
namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IGetWorkflowMetadata
{
    Task<WorkflowMetadata> GetWorkflowMetadataByCodeAsync(string workflowCode, CancellationToken cancellationToken);
}

public record WorkflowMetadata(
    Guid Id,
    string Code,
    //string CurrentStepCode,
	Guid CurrentStepId,
	string CurrentStepType,
	int CurrentStepNumber,
	Guid NextStepId,
	//string Name,
	//string Description,
	IReadOnlyList<StepMetadata> Steps
);
