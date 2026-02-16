using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public interface IDataItem
{
	Guid Id { get; set; }
	string Name { get; set; }
	string Description { get; set ; }
}
