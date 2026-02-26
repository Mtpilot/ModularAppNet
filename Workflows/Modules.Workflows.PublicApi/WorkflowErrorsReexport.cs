using Modules.Common.Domain.Results;
using DomainErrors = Modules.Workflows.Domain.Errors.WorkflowErrors;

namespace Modules.Workflows.PublicApi.Errors;


//SESZH: он предложил это, мне стало интересно, можно ли вообще так. Это как будто единственный способ не дублировать ошибки в баркодинге и потенциально в остальных местах, и, разумеется, сохранить модульность.
/// <summary>
/// Re-export of <see cref="Modules.Workflows.Domain.Errors.WorkflowErrors"/> for cross-module use without referencing Workflows.Domain.
/// </summary>
public static class WorkflowErrors
{
	public static Error NotFound(string workflowCode) =>
		DomainErrors.NotFound(workflowCode);

	public static Error StepNotFound(string stepType) =>
		DomainErrors.StepNotFound(stepType);

	public static Error StepMissMatch(string workflowCode, string stepType, string requestedStepType) =>
		DomainErrors.StepMissMatch(workflowCode, stepType, requestedStepType);

	public static Error WrongStep(string workflowCode, string currentStepType, string endpoint) =>
		DomainErrors.WrongStep(workflowCode, currentStepType, endpoint);

	public static Error NextStepNotFound(string workflowCode, string stepType) =>
		DomainErrors.NextStepNotFound(workflowCode, stepType);

	public static Error AlreadyExists(string workflowCode) =>
		DomainErrors.AlreadyExists(workflowCode);

	public static Error CannotCancel(string workflowCode, string reason) =>
		DomainErrors.CannotCancel(workflowCode, reason);

	public static Error CannotComplete(string workflowCode, string reason) =>
		DomainErrors.CannotComplete(workflowCode, reason);

	public static Error NotOnFinalStep(string workflowCode) =>
		DomainErrors.NotOnFinalStep(workflowCode);

	public static Error PreviousStepNotFound(string workflowCode, string stepType) =>
		DomainErrors.PreviousStepNotFound(workflowCode, stepType);
}
