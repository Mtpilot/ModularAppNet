using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public class DefaultWorkflowDataCollection<TEntity> : IWorkflowDataCollection<TEntity>
{
	public string Name { get; set; }
	public string Description { get; set; }
	public ICollection<TEntity> Collection { get; set; }
}


