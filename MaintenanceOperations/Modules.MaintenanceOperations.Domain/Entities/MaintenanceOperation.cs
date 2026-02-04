
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.MaintenanceOperations.Domain.Entities;

public class MaintenanceOperation
{
	public Guid Id { get; set; }
	public Guid MaintenanceActId { get; set; }
	public DateTime OperationDate { get; set; }
	public string Description { get; set; } = null!;
	public string VehicleNumberPlate { get; set; } = null!;
	public Guid MechanicId { get; set; }
	public decimal Cost { get; set; }
	public List<string> Tasks { get; set; } = new();
}
