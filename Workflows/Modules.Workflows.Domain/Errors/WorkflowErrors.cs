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

	public static Error WrongStep(string code, string currentStepType, string endpoint) =>
		Error.Unexpected($"{ErrorPrefix}.{nameof(WrongStep)}", $"Can't perform '{endpoint}' action on workflow '{code}' on step '{currentStepType}'.");
	public static Error NextStepNotFound(string code, string stepType) =>
		Error.NotFound(
			$"{ErrorPrefix}.{nameof(NextStepNotFound)}",
			$"No next step found for workflow '{code}' from step '{stepType}'");

	public static Error AlreadyExists(string code) =>
		Error.Conflict($"{ErrorPrefix}.{nameof(AlreadyExists)}", $"Workflow with code '{code}' already exists");

	public static Error CannotCancel(string code, string reason) =>
		Error.Validation($"{ErrorPrefix}.{nameof(CannotCancel)}", $"Cannot cancel workflow '{code}': {reason}");

	public static Error CannotComplete(string code, string reason) =>
		Error.Validation($"{ErrorPrefix}.{nameof(CannotComplete)}", $"Cannot complete workflow '{code}': {reason}");

	public static Error NotOnFinalStep(string code) =>
		Error.Validation($"{ErrorPrefix}.{nameof(NotOnFinalStep)}", $"Workflow '{code}' is not on the final step");
	public static Error PreviousStepNotFound(string code, string stepType) =>
		Error.NotFound(
			$"{ErrorPrefix}.{nameof(PreviousStepNotFound)}",
			$"No previous step found for workflow '{code}' from step '{stepType}'");
}
