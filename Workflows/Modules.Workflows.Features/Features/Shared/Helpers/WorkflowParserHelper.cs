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
            // Здесь можно реализовать логику преобразования сущности Workflow в WorkflowResponse
            // Например, маппинг полей, генерация ссылок и т.д.
            // Это позволит держать логику парсинга отдельно от обработчика.
            var response = workflow.ToPartialResponse(
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
        //public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, WorkflowDataCollection<IBaseWorkflowDataDto> tmpData, WorkflowDataCollection<IBaseStepDataDto> tmpStepData)
        //{
        //    var jsonOptions = new JsonSerializerOptions
        //    {
        //        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        //        //WriteIndented = true  // по желанию, для читаемости
        //    };
        //    var response = workflow.ToPartialResponse(
        //        new WorkflowStepDataSchema
        //        {
        //            Version = "1.0",
        //            DataType = "ReceiveGoods",
        //            SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
        //        }, linkService);
        //    var stepData = new WorkflowDataCollection<IBaseStepDataDto> { Name = tmpStepData.Name, Description = tmpStepData.Description, Collection = tmpStepData.Collection };
        //    var workflowData = new WorkflowDataCollection<IBaseWorkflowDataDto> { Name = tmpData.Name, Description = tmpData.Description, Collection = tmpData.Collection };
        //    response.Data = JsonSerializer.Serialize(workflowData, jsonOptions);
        //    response.CurrentStep.Data = JsonSerializer.Serialize(stepData, jsonOptions);
        //    return response;
        //}
    }
    
}
