using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.MaintenanceOperations.Features.Features.Shared.Responses;

public sealed record OperationResponse(
	Guid Id,
	string VehicleNumberPlate,
	string Description,
	Guid MaintenanceActId,
	DateTime OperationDate,
	Guid MechanicId,
	decimal Cost,
	List<string> Tasks);
