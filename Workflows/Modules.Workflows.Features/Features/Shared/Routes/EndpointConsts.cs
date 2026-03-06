namespace Modules.Workflows.Features.Features.Shared.Routes;

internal static class EndpointConsts //SESZH: я увидел, что мы в геттерах всех воркфлоу отправляем просто строку в генератор ссылок. И в мэпперах тоже. В общем, по коду есть строки 
{
	internal const string GetWorkflowTypes = "GetWorkflowTypes";
	internal const string GetActiveWorkflows = "GetActiveWorkflows";
	internal const string GetWorkflows = "GetWorkflows";
	internal const string GetWorkflow = "GetWorkflow";
	internal const string CreateWorkflow = "CreateWorkflow";
	internal const string CancelWorkflow = "CancelWorkflow";
	internal const string CompleteWorkflow = "CompleteWorkflow";
	internal const string NextWorkflowStep = "NextWorkflowStep";
	internal const string PreviousWorkflowStep = "PreviousWorkflowStep";
	internal const string DropMockDb = "DropMockDb";
}