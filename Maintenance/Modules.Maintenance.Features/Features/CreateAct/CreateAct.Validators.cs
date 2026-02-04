using FluentValidation;

namespace Modules.Maintenance.Features.Features.CreateAct;

public class CreateActValidator : AbstractValidator<CreateActRequest>
{
    public CreateActValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");


        RuleFor(x => x.MaintenanceDate)
            .NotEmpty().WithMessage("Maintenance date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Maintenance date cannot be in the past");

        RuleFor(x => x.VehicleId)
            .NotEmpty().WithMessage("Vehicle is required");
    }
}
