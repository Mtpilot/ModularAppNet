using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public interface IWorkflowDataCollection<TEntity> //TEntity - e.g., Invoce(Specification)
{
	string Name { get; set; } //e.g., "Спецификации"

	string Description { get; set; } // Содержит коллекцию документов (в частном случае спецификации)

	ICollection<TEntity> Collection { get; set; }
}
