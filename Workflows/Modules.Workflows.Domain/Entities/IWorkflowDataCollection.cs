using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public interface IWorkflowDataCollection
{
	string Name { get; set; } //e.g., "Спецификации"
	string Description { get; set; }// Содержит коллекцию документов (в частном случае спецификации)

}
public interface IWorkflowDataCollection<TEntity>: IWorkflowDataCollection//TEntity - e.g., Invoce(Specification)
{
	

	ICollection<TEntity> Collection { get; set; }
}
