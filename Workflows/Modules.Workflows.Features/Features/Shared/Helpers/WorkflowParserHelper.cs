using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Features.Features.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Modules.Workflows.Features.Features.Shared.Mappers;

namespace Modules.Workflows.Features.Features.Shared.Helpers
{
    internal static class WorkflowParserHelper
    {
        public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, DefaultWorkflowDataCollection<InvoiceHeader> tmpData, DefaultWorkflowDataCollection<Invoice> tmpStepData)
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
            var stepData = new DefaultWorkflowDataCollection<InvoiceDto> { Name = tmpStepData.Name, Description = tmpStepData.Description, Collection = tmpStepData.Collection.Select(x => new InvoiceDto(x)).ToList() };
            var workflowData = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = tmpData.Name, Description = tmpData.Description, Collection = tmpData.Collection.Select(x => new InvoiceHeaderDto(x)).ToList() };
            response.Data = JsonSerializer.Serialize(workflowData, jsonOptions);
            response.CurrentStep.Data = JsonSerializer.Serialize(stepData, jsonOptions);

            return response;
        }
        public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, DefaultWorkflowDataCollection<InvoiceHeader> tmpData, DefaultWorkflowDataCollection<InvoiceCheckout> tmpStepData)
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
            var stepData = new DefaultWorkflowDataCollection<InvoiceCheckoutDto> { Name = tmpStepData.Name, Description = tmpStepData.Description, Collection = tmpStepData.Collection.Select(x => new InvoiceCheckoutDto(x)).ToList() };
            var workflowData = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = tmpData.Name, Description = tmpData.Description, Collection = tmpData.Collection.Select(x => new InvoiceHeaderDto(x)).ToList() };
            response.Data = JsonSerializer.Serialize(workflowData, jsonOptions);
            response.CurrentStep.Data = JsonSerializer.Serialize(stepData, jsonOptions);
            return response;
        }
    }
    internal class InvoiceDto(Invoice invoice) //SESZH: перенести потом
    {
        public string InvoiceNumber { get; init; } = invoice.InvoiceNumber;
        public string Counterparty { get; init; } = invoice.Counterparty;
        public string ContractNumber { get; init; } = invoice.ContractNumber;
        public DateTime Date { get; init; } = invoice.Date;
        public List<InvoiceLine> Lines { get; init; } = invoice.Lines.ToList();
    }
    internal class InvoiceHeaderDto(InvoiceHeader invoiceHeader)
    {
        public string InvoiceNumber { get; init; } = invoiceHeader.InvoiceNumber;
        public string Counterparty { get; init; } = invoiceHeader.Counterparty;
        public string ContractNumber { get; init; } = invoiceHeader.ContractNumber;
        public DateTime Date { get; init; } = invoiceHeader.Date;
    }
    internal class InvoiceCheckoutDto(InvoiceCheckout invoiceCheckout)
    {
        public string InvoiceNumber { get; init; } = invoiceCheckout.InvoiceNumber;
        public string Counterparty { get; init; } = invoiceCheckout.Counterparty;
        public string ContractNumber { get; init; } = invoiceCheckout.ContractNumber;
        public DateTime Date { get; init; } = invoiceCheckout.Date;
        public int TotalItems { get; init; } = invoiceCheckout.TotalItems;
        public int AcceptedItems { get; init; } = invoiceCheckout.AcceptedItems;
        public int MissingItems { get; init; } = invoiceCheckout.MissingItems;
        public int ExtraItems { get; init; } = invoiceCheckout.ExtraItems;
    }
    
}
