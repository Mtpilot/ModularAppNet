using FluentValidation;
using Modules.MaintenanceOperations.PublicApi.Contracts;

namespace Modules.MaintenanceOperations.Features.Features.CreateOperation;

public class CreateOperationValidator : AbstractValidator<CreateMaintenanceOperationRequest>
{
	public CreateOperationValidator()
	{
		RuleFor(x => x.VehicleNumberPlate)
			.NotEmpty().WithMessage("Vehicle number plate is required")
			.Length(3, 20).WithMessage("Vehicle number plate must be between 3 and 20 characters");

		RuleFor(x => x.Description)
			.NotEmpty().WithMessage("Description is required");

		RuleFor(x => x.OperationDate)
			.NotEmpty().WithMessage("Operation date is required")
			.GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Operation date cannot be in the past");

		RuleFor(x => x.MechanicId)
			.NotEmpty().WithMessage("Mechanic is required");

		RuleFor(x => x.Tasks)
			.NotEmpty().WithMessage("At least one task is required")
			.ForEach(task => task.NotEmpty().WithMessage("Task description cannot be empty"));
	}
}
