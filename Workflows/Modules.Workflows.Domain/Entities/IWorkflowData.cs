using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public interface IWorkflowData<TEntity>
{
	string Name { get; set; }
	string Description { get; set; }
	TEntity Data { get; set; }
}
