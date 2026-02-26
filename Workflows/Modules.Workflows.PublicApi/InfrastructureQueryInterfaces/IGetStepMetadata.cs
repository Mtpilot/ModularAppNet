
namespace Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;

public interface IGetStepMetadata
{
    Task<StepMetadata> GetStepMetadataByCodesAsync(Guid workflowId, string stepCode, CancellationToken cancellationToken);
}
public record StepMetadata(
	Guid Id,
	string StepCode,
	int Order
	//string Name,
	//string Description,
	//string Type,
	//string DataSchema,
	//string Data
	//string Actions
);

