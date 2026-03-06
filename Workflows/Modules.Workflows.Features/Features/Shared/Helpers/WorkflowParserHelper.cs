using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Workflows.Features.Features.Shared.Helpers
{
    internal static class WorkflowParserHelper
    {
        public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, WorkflowDataCollection<IBaseWorkflowDataDto> tmpData, WorkflowDataCollection<IBaseStepDataDto> tmpStepData)
        {
            var response = workflow.ToPartialResponse(
                workflow.CurrentStep(),
                workflow.GetNextStep(), //SESZH: сомнительно тащить его сюда
                new WorkflowStepDataSchema
                {
                    Version = "1.0",
                    DataType = "ReceiveGoods",
                    SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
                },
                linkService);
            response.Data = WorkflowDataCollectionSerializer.Serialize(tmpData);
            response.CurrentStep.Data = WorkflowDataCollectionSerializer.Serialize(tmpStepData);
            return response;
        }
    }
    
}
