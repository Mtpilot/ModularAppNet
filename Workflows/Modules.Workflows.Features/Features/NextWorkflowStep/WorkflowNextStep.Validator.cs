using FluentValidation;
using Modules.Workflows.Features.Features.Shared.Requests;

namespace Modules.Workflows.Features.Features.NextWorkflowStep;

public sealed class WorkflowNextStepBodyRequestValidator : AbstractValidator<WorkflowNextStepBodyRequest>
{
	public WorkflowNextStepBodyRequestValidator()
	{
		RuleFor(request => request.Data)
			.NotNull()
			.WithMessage("Data is required");
	}
}
