using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;


using Modules.Workflows.Domain.Serializers;


namespace Modules.Workflows.Domain.Entities;

public class Workflow//<TEntity, TStepEntity>
{
	public required Guid Id { get; set; }
	public required string Code { get; set; }

	//public required string TypeCode { get; set; }

	public required string Name { get; set; }
	public required string Description { get; set; }

	public required bool IsActive { get; set; }
	public required int CurrentStepNumber { get; set; }
	public required WorkflowStepType CurrentStepType { get; set; }
	public required WorkflowType Type {get; set;}
	public required List<WorkflowStep> Steps { get; set; }

	//public required List<WorkflowAction> Actions { get; set; }

	//SESZH: убрал в таком виде, добавляю в виде списка Guid, посмотрим, что будет с поиском по разным таблицам. UPD: пока не актуально

	//TODO: Как связаны Data и WorkflowDataItems, в частном случае приемки тут должне быть массив содержащий один элемент - это спецификация с полями шапки спецификации
	[NotMapped]
	public IWorkflowDataCollection Data { get; set; }

	//public IWorkflowDataCollection<TData> Data
	//{
	//	get => string.IsNullOrEmpty(DataJson)
	//		? null!
	//		: JsonSerializer.Deserialize<WorkflowDataItemsCollection<TData>>(DataJson, DataItemSerializer.DataItemSeializerOptions)!;
	//	set => DataJson = value == null ? "{}" : JsonSerializer.Serialize(value, DataItemSerializer.DataItemSeializerOptions);
	//}
	//public string DataJson { get; set; } = string.Empty;

	//public required List<Guid> DataGuids {  get; set; }

	/// <summary>
	/// Строковая копия Data для хранения в БД. Парно сериализуется с <see cref="Data"/> через <see cref="Serializers.WorkflowDataSerializer"/>.
	/// В Features для parse использовать <see cref="Serializers.WorkflowDataSerializer.Deserialize{TEntity}(string?)"/>.
	/// </summary>
	//public string DataJson { get; set; } = "{}";

	/*
	[NotMapped]
	public IWorkflowData<TEntity> Data
	{
		get => WorkflowDataSerializer.Deserialize<TEntity>(DataJson) ?? new DefaultWorkflowData<TEntity> { Name = "", Description = "", Data = default! };
		set => DataJson = WorkflowDataSerializer.Serialize(value);
	}
	*/
	#region Steps
	public string GetCacheKey() => $"workflow:{Code}";

	//public WorkflowStep CurrentStep() => GetStepByType(CurrentStepType) ?? throw new InvalidOperationException($"Current step type '{CurrentStepType}' not found in workflow '{Code}'.");
	public WorkflowStep CurrentStep() => GetStepByOrder(CurrentStepNumber) ?? throw new InvalidOperationException($"Current step with order '{CurrentStepNumber}' not found in workflow '{Code}'.");
	public WorkflowStep? GetStepByType(WorkflowStepType type) => Steps.FirstOrDefault(step => step.Type == type);
	public WorkflowStep? GetStepByOrder(int order) =>
		Steps.FirstOrDefault(step => step.Order == order);

	public WorkflowStep? GetFirstStep()
	{
		WorkflowStep? firstStep = null;
		//SESZH
		firstStep = Steps.MinBy(x=> x.Order);

		foreach (var step in Steps)
		{
			if (firstStep == null || step.Order < firstStep.Order)
			{
				firstStep = step;
			}
		}
		return firstStep;
	}

	public WorkflowStep? GetNextStep(int currentOrder)
	{
		WorkflowStep? nextStep = null;
		//SESZH: linq мне показался очевидным, интересно, почему не так?
		//nextStep = Steps.Where(x=> x.Order> currentOrder).MinBy(x=> x.Order);

		foreach (var step in Steps)
		{
			if (step.Order > currentOrder && (nextStep == null || step.Order < nextStep.Order))
			{				
				nextStep = step;
				
			}
		}
		return nextStep;
	}

	public WorkflowStep? GetPreviousStep(int currentOrder)
	{
		WorkflowStep? previousStep = null;
		//SESZH: linq мне показался очевидным, интересно, почему не так?
		//previousStep = Steps.Where(x => x.Order < currentOrder).MaxBy(x => x.Order);

		foreach (var step in Steps)
		{
			if (step.Order < currentOrder && (previousStep == null || step.Order > previousStep.Order))
			{								
				previousStep = step;		
			}
		}
		return previousStep;
	}
	public void SetStepNumber(int number) //SESZH: возможно, сделать просто навигацию вперед-назад? Тогда уже этому методу нужно будет заниматься валидацией перехода
	{
		if (CurrentStepNumber == 1 && number > CurrentStepNumber) //SESZH: подумать над переходами и состоянием IsActive
		{
			IsActive = true;
		}
		CurrentStepNumber = number;
	}
	public bool IsFinalStep()
	{
		var currentStep = CurrentStep();
		if (currentStep is null)
		{
			return false;
		}

		var nextStep = GetNextStep(currentStep.Order);
		return nextStep is null;
	}
	#endregion Steps

	#region Workflow Management
	public void Cancel()
	{
		IsActive = false;
	}

	public void Complete()
	{
		IsActive = false;
	}
	#endregion Workflow Management

	public override string ToString()
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Workflow: {Name} (Code: {Code}, Active: {IsActive})");
		sb.AppendLine($"Description: {Description}");
		sb.AppendLine("Steps:");
		foreach (var step in Steps)
		{
			sb.AppendLine($"  - Step: {step.Name} (Type: {step.Type}, Order: {step.Order})");
		}
		return sb.ToString();
	}
}



//public class WorkflowAction
//{
//	public required string Type { get; set; }
//	public required string Name { get; set; }
//	public required string Description { get; set; }
//}




public record WorkflowDataItemsCollection<T> : IWorkflowDataCollection<T> //SESZH: пусть пока будет старая версия один тип у всех
{
	public required string Name { get; set; }
	public required string Description { get; set; }
	public ICollection<T> Collection { get; set; } = Array.Empty<T>();
}
