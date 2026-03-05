using Modules.Common.Domain.Collections;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowDataCollection<TEntity> : IWorkflowDataCollection<TEntity> //SESZH: подумать над расположением. Изначально это была модель как бы из бд
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public ICollection<TEntity> Collection { get; set; } = new List<TEntity>();
}
