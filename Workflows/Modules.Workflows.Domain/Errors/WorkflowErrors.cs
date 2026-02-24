using Modules.Common.Domain.Results;

namespace Modules.Workflows.Domain.Errors;

public static class WorkflowErrors
{
	private const string ErrorPrefix = "Workflows";

	public static Error NotFound(string workflowCode) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(NotFound)}", $"Workflow with code '{workflowCode}' not found");

	public static Error StepNotFound(string stepType) =>
		Error.NotFound($"{ErrorPrefix}.{nameof(StepNotFound)}", $"Workflow step '{stepType}' not found");

	public static Error StepMissMatch(string workflowCode, string stepType, string requestedStepType) =>
	Error.Unexpected($"{ErrorPrefix}.{nameof(StepMissMatch)}", $"Can't transfer workflow '{workflowCode}' to step '{requestedStepType}'. Expating step is '{stepType}' ");

	public static Error WrongStep(string workflowCode, string currentStepType, string endpoint) =>
		Error.Unexpected($"{ErrorPrefix}.{nameof(WrongStep)}", $"Can't perform '{endpoint}' action on workflow '{workflowCode}' on step '{currentStepType}'.");
	public static Error NextStepNotFound(string workflowCode, string stepType) =>
		Error.NotFound(
			$"{ErrorPrefix}.{nameof(NextStepNotFound)}",
			$"No next step found for workflow '{workflowCode}' from step '{stepType}'");

	public static Error AlreadyExists(string workflowCode) =>
		Error.Conflict($"{ErrorPrefix}.{nameof(AlreadyExists)}", $"Workflow with code '{workflowCode}' already exists");

	public static Error CannotCancel(string workflowCode, string reason) =>
		Error.Validation($"{ErrorPrefix}.{nameof(CannotCancel)}", $"Cannot cancel workflow '{workflowCode}': {reason}");

	public static Error CannotComplete(string workflowCode, string reason) =>
		Error.Validation($"{ErrorPrefix}.{nameof(CannotComplete)}", $"Cannot complete workflow '{workflowCode}': {reason}");

	public static Error NotOnFinalStep(string workflowCode) =>
		Error.Validation($"{ErrorPrefix}.{nameof(NotOnFinalStep)}", $"Workflow '{workflowCode}' is not on the final step");
	public static Error PreviousStepNotFound(string workflowCode, string stepType) =>
		Error.NotFound(
			$"{ErrorPrefix}.{nameof(PreviousStepNotFound)}",
			$"No previous step found for workflow '{workflowCode}' from step '{stepType}'");
}
