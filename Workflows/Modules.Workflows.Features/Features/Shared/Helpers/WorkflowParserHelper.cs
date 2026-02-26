using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.PublicApi.Responses;
using System.Text.Json;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Workflows.Features.Features.Shared.Helpers
{
    internal static class WorkflowParserHelper
    {
        public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, DefaultWorkflowDataCollection<InvoiceHeaderDto> tmpData, DefaultWorkflowDataCollection<InvoiceDto> tmpStepData)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //WriteIndented = true  // по желанию, для читаемости
            };
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
            var stepData = new DefaultWorkflowDataCollection<InvoiceDto> { Name = tmpStepData.Name, Description = tmpStepData.Description, Collection = tmpStepData.Collection };
            var workflowData = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = tmpData.Name, Description = tmpData.Description, Collection = tmpData.Collection };
            response.Data = JsonSerializer.Serialize(workflowData, jsonOptions);
            response.CurrentStep.Data = JsonSerializer.Serialize(stepData, jsonOptions);

            return response;
        }
        public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, DefaultWorkflowDataCollection<InvoiceHeaderDto> tmpData, DefaultWorkflowDataCollection<InvoiceCheckoutDto> tmpStepData)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                //WriteIndented = true  // по желанию, для читаемости
            };
            var response = workflow.ToPartialResponse(
                new WorkflowStepDataSchema
                {
                    Version = "1.0",
                    DataType = "ReceiveGoods",
                    SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
                }, linkService);
            var stepData = new DefaultWorkflowDataCollection<InvoiceCheckoutDto> { Name = tmpStepData.Name, Description = tmpStepData.Description, Collection = tmpStepData.Collection };
            var workflowData = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = tmpData.Name, Description = tmpData.Description, Collection = tmpData.Collection };
            response.Data = JsonSerializer.Serialize(workflowData, jsonOptions);
            response.CurrentStep.Data = JsonSerializer.Serialize(stepData, jsonOptions);
            return response;
        }
    }
    
}
