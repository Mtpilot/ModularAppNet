using FluentValidation;

namespace Modules.Maintenance.Features.Features.AddOperationToAct;

public class AddOperationToActValidator : AbstractValidator<AddOperationToActRequest>
{
	public AddOperationToActValidator()
	{
		RuleFor(x => x.ActId)
			.NotEmpty().WithMessage("Act ID is required");

		RuleFor(x => x.VehicleNumberPlate)
			.NotEmpty().WithMessage("Vehicle number plate is required")
			.Length(3, 20).WithMessage("Vehicle number plate must be between 3 and 20 characters");

		RuleFor(x => x.Description)
			.NotEmpty().WithMessage("Description is required");

		RuleFor(x => x.OperationDate)
			.NotEmpty().WithMessage("Operation date is required")
			.GreaterThanOrEqualTo(_ => DateTime.UtcNow).WithMessage("Operation date cannot be in the past");

		RuleFor(x => x.MechanicId)
			.NotEmpty().WithMessage("Mechanic is required");

		RuleFor(x => x.Cost)
			.GreaterThanOrEqualTo(0).WithMessage("Cost must be greater than or equal to 0");

		RuleForEach(x => x.Tasks)
			.NotEmpty().WithMessage("Task description cannot be empty");
	}
}
