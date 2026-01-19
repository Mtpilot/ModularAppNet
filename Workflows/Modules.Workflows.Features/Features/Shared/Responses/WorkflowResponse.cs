using System.Text.Json;
using Modules.Common.API.Abstractions.Links;


namespace Modules.Workflows.Features.Features.Shared.Responses;


public abstract record WorkflowBaseInfo (string Code, string TypeCode, string Name, string Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public required JsonElement Data { get; set; }
}

public sealed record WorkflowShortInfoResponse (string Code, string TypeCode, string Name, string Description) : WorkflowBaseInfo(Code, TypeCode, Name, Description)
{
	public required string CurrentStepType { get; set; }
	public required string CurrentStepName { get; set; }

	public required List<Link> Links { get; set; }
}


public sealed record WorkflowResponse (string Code, string TypeCode, string Name, string Description) : WorkflowBaseInfo(Code, TypeCode, Name, Description)
{
	public required WorkflowCurrentStep CurrentStep { get; set; }

	public required List<WorkflowStepShortResponse> WorkflowSteps { get; set; }
}


public sealed record WorkflowStepShortResponse (string Type, string Name, int Order); //Сканировать, Проверить, Принять (Scan, Verify, Accept)


public sealed record WorkflowCurrentStep(string Type, string Name, string Description)
{
	public required WorkflowStepDataSchema DataSchema { get; set; }
	public required JsonElement Data { get; set; }
	public required Actions Actions { get; set; }
}


public sealed record Actions
{
	public required List<Link> Links { get; set; }
	public Link? NextStep { get; set; }
}

public sealed record WorkflowStepDataSchema
{
	public required string Version { get; set; }
	public required string DataType { get; set; }
	public required string SchemaJson { get; set; }
}
