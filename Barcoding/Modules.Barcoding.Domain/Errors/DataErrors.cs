using Modules.Common.Domain.Results;

namespace Modules.Barcoding.Domain.Errors;

public static class DataErrors
{
	private const string ErrorPrefix = "Barcoding";

	public static Error StepDataNotFound(string workflowCode, string stepCode) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(StepDataNotFound)}", $"Step data for workflow '{workflowCode}' and step '{stepCode}' not found");

	public static Error NextStepDataNotFound(string workflowCode, string stepCode) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(NextStepDataNotFound)}", $"Next step data for workflow '{workflowCode}' and step '{stepCode}' not found");
}
