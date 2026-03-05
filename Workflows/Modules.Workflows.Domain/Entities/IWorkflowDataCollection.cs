using Modules.Common.Domain.Collections;

namespace Modules.Workflows.Domain.Entities;

/// <summary>
/// Специализация коллекции данных для Workflow.
/// Наследует базовую абстракцию из Common.
/// </summary>
public interface IWorkflowDataCollection : ICommonDataCollection;

/// <summary>
/// Типизированная коллекция данных Workflow.
/// Наследует базовую абстракцию из Common.
/// </summary>
/// <typeparam name="TEntity">Тип элемента коллекции (например, InvoiceHeaderDto, InvoiceDto).</typeparam>
public interface IWorkflowDataCollection<TEntity> : ICommonDataCollection<TEntity>, IWorkflowDataCollection; //TEntity - e.g., Invoce(Specification)
