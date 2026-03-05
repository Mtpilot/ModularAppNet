using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.MockInfrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Modules.Workflows.Features.QueryHandlers
{
    public class GetStepMetadata(WorkflowsDbContext context) : IGetStepMetadata
    {
        public async Task<StepMetadata> GetStepMetadataByCodesAsync(Guid workflowId, string stepCode, CancellationToken cancellationToken)
        {
            var step = await context.WorkflowSteps.FirstOrDefaultAsync(x => x.WorkflowId == workflowId && x.StepCode == stepCode, cancellationToken);
            if (step == null)
            {
                return null;
            }
            return new StepMetadata(step.Id, step.StepCode, step.Order, step.Type.ToString());
        }
    }
}
