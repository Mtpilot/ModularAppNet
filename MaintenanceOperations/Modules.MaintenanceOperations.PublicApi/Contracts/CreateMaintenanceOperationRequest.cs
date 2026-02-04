using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.MaintenanceOperations.PublicApi.Contracts;

public record CreateMaintenanceOperationRequest
	(
Guid MaintenanceActId,
DateTime OperationDate,
string Description,
string VehicleNumberPlate,
Guid MechanicId,
decimal Cost,
List<string> Tasks
	);
