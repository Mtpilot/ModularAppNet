using FluentValidation;
using Modules.Workflows.Features.Features.Shared.Requests;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

internal sealed class WorkflowNextStepBodyRequestValidator : AbstractValidator<WorkflowNextStepBodyRequest>
{
	public WorkflowNextStepBodyRequestValidator()
	{
		

		RuleFor(request => request.Data)
			.NotNull();
	}
}
