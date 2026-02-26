using FluentValidation;
using Modules.Workflows.PublicApi.Requests;

namespace Modules.Workflows.Features.Features.CreateWorkflow;

public sealed class CreateWorkflowRequestValidator : AbstractValidator<CreateWorkflowRequest>
{
	public CreateWorkflowRequestValidator()
	{
		RuleFor(request => request.TypeCode)
			.NotEmpty()
			.WithMessage("TypeCode is required");

		RuleFor(request => request.Name)
			.NotEmpty()
			.WithMessage("Name is required");

		RuleFor(request => request.Description)
			.NotEmpty()
			.WithMessage("Description is required");

		RuleFor(request => request.Data)
			.Custom((data, context) =>
			{
				if (data.ValueKind == System.Text.Json.JsonValueKind.Null || 
				    data.ValueKind == System.Text.Json.JsonValueKind.Undefined)
				{
					context.AddFailure("Data is required");
				}
			});
	}
}
