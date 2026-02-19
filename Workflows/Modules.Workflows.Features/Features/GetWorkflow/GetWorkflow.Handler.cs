using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Features.Features.Shared.Mappers;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.GetWorkflow;

internal interface IGetWorkflowHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken);
}


internal sealed class GetWorkflowHandler(
	ILogger<GetWorkflowHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService) : IGetWorkflowHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

		// Implementation goes here
		//const string workflowTypeCode = "ReceiveGoods";
		//const string workflowCode = "123";
		//const string type = "Verify";
		//var nextStepLink = linkService.Generate("WorkflowNextStep", new { code = workflowCode, type }, "Move to Next Step", HttpMethod.PATCH)

        	var workflow = await context.Workflows
        		.Include(w=> w.Type)
        .Include(w => w.Steps)
        	.ThenInclude(s => s.Actions)
        .FirstAsync(w => w.Code == workflowCode, cancellationToken);


       // workflow.Data = new DefaultWorkflowDataCollection<InvoiceHeader>()
       // {
       //     Name = "InvoiceHeaders",
       //     Description = "Collection of invoice headers",
       //     Collection = new List<InvoiceHeader>()
       //     { 
       //         new InvoiceHeader
       //         {
       //             InvoiceId = "INV-2024-001",
       //             Counterparty = "ООО Поставщик",
       //             Contract = "Договор-2024"
       //         }
       //     }
       // };


        //	var links = await context.WorkflowDataLinks
        //.Where(l => workflow.DataGuids.Contains(l.DataId))
        //.ToListAsync(cancellationToken);

        //		var groups = links.GroupBy(l => l.DataType);
        //
        //foreach(var group in groups)
        //		{
        //			switch (group.Key)
        //			{
        //#pragma warning disable
        //				case "Item": workflow.Data.Name = "DataItems";  workflow.Data.Collection = await context.WorkflowDataItems.Where(x => group.Select(y => y.DataId).Contains(x.Id)).Select(item => new Dictionary<string, object>(JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(item)), StringComparer.OrdinalIgnoreCase)).ToListAsync(cancellationToken); break;
        //#pragma warning restore
        //				default: throw new NotSupportedException();
        //			}
        //		}

        //var currentStepData = workflow.CurrentStep().Data;

        var tmpData = await GetMockInvoiceHeadersFromInMemoryDb(context, workflow.Id, cancellationToken);
        var wf = workflow.ConvertWorkflowToResponse(linkService, tmpData);




		return wf;
	}
    private async Task<DefaultWorkflowDataCollection<InvoiceHeader>> GetMockInvoiceHeadersFromInMemoryDb(WorkflowsDbContext context, Guid workflowId, CancellationToken cancellationToken)
    {
        return new DefaultWorkflowDataCollection<InvoiceHeader> { Name = "InvoiceHeaders", Description = "Collection of invoice headers", Collection = await context.InvoiceHeaders.Where(x => x.WorkflowId == workflowId).ToListAsync(cancellationToken) };
    }
}

internal static class WorkflowParserHelper
{
    public static WorkflowResponse ConvertWorkflowToResponse(this Workflow workflow, ILinkService linkService, DefaultWorkflowDataCollection<InvoiceHeader> tmpData)
    {
        // Здесь можно реализовать логику преобразования сущности Workflow в WorkflowResponse
        // Например, маппинг полей, генерация ссылок и т.д.
        // Это позволит держать логику парсинга отдельно от обработчика.
        var response = workflow.ToResponse(
            new WorkflowStepDataSchema
            {
                Version = "1.0",
                DataType = "ReceiveGoods",
                SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
            },
            linkService);
            response.Data = JsonSerializer.SerializeToElement(tmpData);
        return response;
    }
}
    /// <summary>
    /// Example of creating JsonElement with sample data for CurrentStep.Data field.
    /// This demonstrates how to populate Data with arbitrary JSON structures including arrays and nested objects.
    /// </summary>
//	private static JsonElement CreateSampleData()
//	{
//		var sampleData = new
//		{
//			invoiceNumber = "INV-2024-001",
//			date = "2024-01-15",
//			supplier = new
//			{
//				name = "ООО Поставщик",
//				inn = "1234567890"
//			},
//			items = new[]
//			{
//				new
//				{
//					itemCode = "PROD-001",
//					name = "Товар 1",
//					quantity = 10,
//					unit = "шт",
//					price = 1500.50m,
//					tags = new[] { "urgent", "fragile" }
//				},
//				new
//				{
//					itemCode = "PROD-002",
//					name = "Товар 2",
//					quantity = 5,
//					unit = "шт",
//					price = 2300.00m,
//					tags = new[] { "standard" }
//				}
//			},
//			metadata = new
//			{
//				source = "warehouse",
//				priority = "high",
//				notes = new[]
//				{
//					"Требуется проверка качества",
//					"Срочная доставка"
//				}
//			}
//		};
//
//		var jsonString = JsonSerializer.Serialize(sampleData);
//		using var document = JsonDocument.Parse(jsonString);
//		return document.RootElement.Clone();
//	}
//}

//var wf = new WorkflowResponse(workflowCode, workflowTypeCode, "Приемка по накладной", "Приемка по каждой строчки накладной")
//{
//	Data = JsonDocument.Parse("{ \"InvoiceId\": \"string\", \"Сounterparty\": \"string\", \"Contract\": \"string\" }").RootElement,
//	DataSchema = new WorkflowStepDataSchema
//	{
//		Version = "1.0",
//		DataType = "ReceiveGoods",
//		SchemaJson = "{ 'InvoiceId': 'string', 'Сounterparty': 'string', 'Contract': 'string' }",
//	},
//
//	CurrentStep = new WorkflowCurrentStep("Scan", "Шаг сканирования", "")
//	{
//	Actions = new Actions
//	{
//		Links = new List<Link>
//		{
//			new Link("/api/workflows/{code}/data/{itemId}/","Increment Qty", HttpMethod.POST), 
//			new Link("/api/workflows/{itemId}/", "Add Line", HttpMethod.PUT),
//			new Link("/api/workflows/{code}/data/", "Send Barcode Pack", HttpMethod.POST)
//			//scanned pack, next/prevsteps, 
//		},
//		NextStep = nextStepLink
//	},
//		DataSchema = new WorkflowStepDataSchema
//		{
//			Version = "1.0",
//			DataType = "Invoice",
//			SchemaJson = "{ 'type': 'object', 'properties': { 'invoiceNumber': { 'type': 'string' }, 'items': { 'type': 'array', 'items': { 'type': 'object', 'properties': { 'itemCode': { 'type': 'string' }, 'quantity': { 'type': 'integer' } }, 'required': ['itemCode', 'quantity'] } } }, 'required': ['invoiceNumber', 'items'] }",
//		},
//		Data = CreateSampleData()
//
//	},
//	WorkflowSteps = new List<WorkflowStepShortResponse>
//	{
//		new WorkflowStepShortResponse("Scan", "Шаг сканирования", 1),
//		new WorkflowStepShortResponse("Verify", "Шаг проверки", 2),
//		new WorkflowStepShortResponse("Complete", "Шаг завершения", 3)
//	}
//};
