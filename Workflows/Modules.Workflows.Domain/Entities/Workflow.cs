using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Modules.Workflows.Domain.Entities;

public class Workflow<TData>	
{
	public required string Code { get; set; }

	public required string Name { get; set; }
	public required string Description { get; set; }

	public required bool IsActive { get; set; }

	public required string CurrentStepType { get; set; }
	public required List<WorkflowStep> Steps { get; set; }

	//public required List<WorkflowAction> Actions { get; set; }
	public required IWorkflowDataCollection<TData> Data { get; set; }

	#region Steps
	public string GetCacheKey() => $"workflow:{Code}";

	public WorkflowStep CurrentStep() => GetStepByType(CurrentStepType) ?? throw new InvalidOperationException($"Current step type '{CurrentStepType}' not found in workflow '{Code}'.");
	public WorkflowStep? GetStepByType(string type) => Steps.FirstOrDefault(step => step.Type.Equals(type, StringComparison.OrdinalIgnoreCase));


	public WorkflowStep? GetFirstStep()
	{
		WorkflowStep? firstStep = null;
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
		foreach (var step in Steps)
		{
			if (step.Order < currentOrder && (previousStep == null || step.Order > previousStep.Order))
			{								
				previousStep = step;		
			}
		}
		return previousStep;
	}
	#endregion Steps


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


public class WorkflowStep //Сканировать, Проверить, Принять
{
	public required string Type { get; set; } //Scan, Verify, Accept
	public required string Name { get; set; }
	public required int Order { get; set; }
	public required string Description { get; set; }

	public required List<WorkflowStepAction> Actions { get; set; }
}


public class WorkflowStepAction
{
	public required string Type { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
}


//public class WorkflowAction
//{
//	public required string Type { get; set; }
//	public required string Name { get; set; }
//	public required string Description { get; set; }
//}


public interface IWorkflowDataCollection<TEntity> //TEntity - e.g., Invoce(Specification)
{
	string Name { get; set; } //e.g., "Спецификация"

	string Description { get; set; }

	ICollection<TEntity> Collection { get; set; } 
}
