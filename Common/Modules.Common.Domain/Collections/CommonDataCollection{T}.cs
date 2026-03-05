namespace Modules.Common.Domain.Collections;

/// <summary>
/// Базовая реализация типизированной коллекции данных.
/// Модули используют наследников (например, WorkflowDataCollection в Workflows).
/// </summary>
/// <typeparam name="TEntity">Тип элемента коллекции.</typeparam>
public class CommonDataCollection<TEntity> : ICommonDataCollection<TEntity>
{
	public string Name { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;
	public ICollection<TEntity> Collection { get; set; } = new List<TEntity>();
}
