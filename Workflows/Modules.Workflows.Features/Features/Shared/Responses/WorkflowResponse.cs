using Modules.Common.API.Abstractions.Links;


namespace Modules.Workflows.Features.Features.Shared.Responses;

public sealed record WorkflowResponse (string Code, string Name, string Description)
{
	public required WorkflowCurrentStep CurrentStep { get; set; }

	public required List<WorkflowStepShortResponse> WorkflowSteps { get; set; }
}


public sealed record WorkflowStepShortResponse (string Type, string Name, int Order); //Сканировать, Проверить, Принять (Scan, Verify, Accept)


public sealed record WorkflowCurrentStep(string Type, string Name, string Description)
{
	public required Actions Actions { get; set; }
}


public sealed record Actions
{
	public required List<Link> Links { get; set; }
	public Link? NextStep { get; set; }
}
