using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.MockInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Modules.Workflows.Features.QueryHandlers
{
    internal class GetWorkflowMetadata(WorkflowsDbContext context): IGetWorkflowMetadata
    {
        public async Task<WorkflowMetadata> GetWorkflowMetadataByCodeAsync(string workflowCode, CancellationToken cancellationToken)
        {
            var workflow = await context.Workflows.Include(x => x.Steps).FirstOrDefaultAsync(x => x.Code == workflowCode, cancellationToken);
            if (workflow == null)
            {
                return null;
            }
            return new WorkflowMetadata(workflow.Id, workflow.Code, workflow.CurrentStep().Id, workflow.GetCurrentStepType(), workflow.CurrentStepNumber, workflow.GetNextStep().Id, workflow.Steps.Select(x => new StepMetadata(x.Id, x.StepCode, x.Order, x.Type.ToString())).ToList());
        }
    }
}
