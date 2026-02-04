using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Maintenance.Domain.Entities;

public class MaintenanceAct
{
	public Guid Id { get; set; }
	public Guid VehicleId { get; set; }
	public DateTime MaintenanceDate { get; set; }
	public string Description { get; set; } = null!;
}
