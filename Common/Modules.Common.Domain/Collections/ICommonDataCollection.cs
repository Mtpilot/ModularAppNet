namespace Modules.Common.Domain.Collections;

/// <summary>
/// Базовый интерфейс коллекции данных с метаданными.
/// Модули используют наследников для своих доменных коллекций (например, WorkflowDataCollection).
/// </summary>
public interface ICommonDataCollection
{
	string Name { get; set; }
	string Description { get; set; }
}
