using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public class DefaultWorkflowData<TEntity> : IWorkflowData<TEntity>
{
	public string Name { get; set; }
	public string Description { get; set; }
	public TEntity Data { get; set; }
}
