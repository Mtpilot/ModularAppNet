using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Modules.Workflows.Features.Features.Shared.Requests;

namespace Modules.Workflows.Features.Features.PreviousWorkflowStep;

public sealed class WorkflowPreviousStepBodyRequestValidator : AbstractValidator<WorkflowPreviousStepBodyRequest>
{
	public WorkflowPreviousStepBodyRequestValidator()
	{
		RuleFor(request => request.Data)
			.NotNull()
			.WithMessage("Data is required");
	}
}

