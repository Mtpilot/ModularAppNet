namespace Modules.Common.Domain.Collections;

/// <summary>
/// Типизированная коллекция данных.
/// </summary>
/// <typeparam name="TEntity">Тип элемента коллекции.</typeparam>
public interface ICommonDataCollection<TEntity> : ICommonDataCollection
{
	ICollection<TEntity> Collection { get; set; }
}
