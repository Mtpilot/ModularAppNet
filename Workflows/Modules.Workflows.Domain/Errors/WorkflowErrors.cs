using Modules.Common.Domain.Results;

namespace Modules.Workflows.Domain.Errors;

public static class WorkflowErrors
{
	private const string ErrorPrefix = "Workflows";

	public static Error NotFound(string code) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"Workflow with code '{code}' not found");

	public static Error StepNotFound(string stepType) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(StepNotFound)}", $"Workflow step '{stepType}' not found");

	public static Error StepMissMatch(string code, string stepType, string requestedStepType) =>
	Error.Unexpected($"{ErrorPrefix}.{nameof(StepMissMatch)}", $"Can't transfer workflow '{code}' to step '{requestedStepType}'. Expating step is '{stepType}' ");

	public static Error NextStepNotFound(string code, string stepType) =>
		Error.NotFound(
			$"{ErrorPrefix}.{nameof(NextStepNotFound)}",
			$"No next step found for workflow '{code}' from step '{stepType}'");
}
