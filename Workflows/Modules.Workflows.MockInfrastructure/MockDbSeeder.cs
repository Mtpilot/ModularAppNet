using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Serializers;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.MockInfrastructure;

public static class MockDbSeeder
{
	public static void Seed(WorkflowsDbContext context)
	{
		SeedWorkflows(context);
	}
	public static void SeedWorkflows(WorkflowsDbContext context)
	{
		var workflowId = Guid.NewGuid();
		var workflow = new Workflow<IDataItem>
		{
			Id = workflowId,
			Code = "TestWorkflow",
			Name = "Test Workflow",
			Description = "A test workflow for demonstration purposes.",
			IsActive = true,
			CurrentStepType = "Scan",
			TypeCode = "TEST",
			Data = new WorkflowDataItemsCollection<IDataItem> { Name = "Items", Description = "Empty Descr", Collection = SeedDataItems(context).Cast<IDataItem>().ToList() },
			WorkflowDataItems = SeedDataItems(context, workflowId),
			//DataGuids = SeedDataItems(context),
			Steps = SeedSteps(workflowId),
		};
		context.Workflows.Add(workflow);
		context.SaveChanges();
	}
#pragma warning disable MA0051
	private static List<WorkflowStep> SeedSteps(Guid workflowId)
	{
#pragma warning restore MA0051
		var step1Id = Guid.NewGuid();
		var step2Id = Guid.NewGuid();
		return new List<Domain.Entities.WorkflowStep>
			{
				new WorkflowStep
				{
					Id = Guid.NewGuid(),
					WorkflowId = workflowId,
					 Type = "Scan",
					 Description = "Scanning the items",
					 Name = "Scan",
					 Order = 0,
					Actions = new List<WorkflowStepAction>
					{
						new WorkflowStepAction
						{
						  Id = Guid.NewGuid(),
						  StepId = step1Id,
						  Name = "SendPackage",
						  Type = "SendPackage",
						  HttpMethod = "POST",
						  Description = "Send scanned barcodes with quantities",
						  Endpoint = "SendScannedBarcodes",
						  RouteParams = new Dictionary<string, object> {["subpoint"] = "data"},

						},
						new WorkflowStepAction
						{
							Id = Guid.NewGuid(),
							StepId = step1Id,
							Name = "GoToNextStep",
							Type = "GoToNextStep",
							HttpMethod = "PATCH",
							Description = "Go to next step",
							Endpoint = "WorkflowNextStep",
							RouteParams = [],
						}
					}
				},
				new WorkflowStep
				{
					Id = Guid.NewGuid(),
					WorkflowId = workflowId,
					Name = "CheckoutName",
					Description = "Checking the scanned items",
					Order = 1,
					Type = "Checkout",
					Actions = new List<WorkflowStepAction>
					{
							new WorkflowStepAction
						{
							Id = Guid.NewGuid(),
							StepId = step2Id,
							Name = "GoToNextStepName",
							Type = "GoToNextStep",
							HttpMethod = "PATCH",
							Description = "Go to next step",
							Endpoint = "WorkflowNextStep",
							RouteParams = [],
						},
							new WorkflowStepAction
						{
							Id = Guid.NewGuid(),
							StepId = step2Id,
							Name = "GoToPreviousStep",
							Type = "GoToPreviousStep",
							HttpMethod = "PATCH",
							Description = "Go to Previous step",
							Endpoint = "WorkflowPrevStep",
							RouteParams = [],
						}
					}
				}
		};
	}
	public static List<DataItemInventory> SeedDataItems(WorkflowsDbContext context)
	{
		var items = SeedItems();
		context.WorkflowDataItemInventory.AddRange(items);
		context.WorkflowDataLinks.AddRange(items.Select(x => new WorkflowDataLink { DataId = x.Id, DataType = "Item" }));
		//context.SaveChanges();
		//return items.Select(x => x.Id).ToList();
		return items;
	}
	public static List<WorkflowDataItem> SeedDataItems(WorkflowsDbContext context, Guid workflowId)
	{
		var items = SeedItems();

		var workflowDataItems = items.Select(item => new WorkflowDataItem
		{
			Id = Guid.NewGuid(),
			WorkflowId = workflowId,
			Name = item.Name,
			Description = item.Description,
			Code = item.ItemCode,
			Quantity = item.Quantity,
			Discriminator = "inventory",
			JsonData = JsonSerializer.Serialize(item, DataItemSerializer.DataItemSeializerOptions),
		}).ToList();

		context.WorkflowDataItems.AddRange(workflowDataItems);
		//context.SaveChanges();

		return workflowDataItems;
	}
	private static List<DataItemInventory> SeedItems()
	{
		return new List<DataItemInventory>
		{
			new DataItemInventory
			{
				Id = Guid.NewGuid(),
				Name = "Товар 1",
				Description = "",
				ItemCode = "PROD-001",
				Quantity = 10,
				Units = "шт",
				Price = 1500.50m,
#pragma warning disable CA1861 //для моков пойдет
				Tags = new[] { "urgent", "fragile" },
#pragma warning restore CA1861
			},
			new DataItemInventory
			{
				Id = Guid.NewGuid(),
				Name = "Товар 2",
				Description = "",
				ItemCode = "PROD-002",
				Quantity = 5,
				Units = "шт",
				Price = 2300.50m,
#pragma warning disable CA1861 //для моков пойдет
				Tags = new[] { "standard" },
#pragma warning restore CA1861
			}
		};
	}
}
	//private static Workflow<Dictionary<string, object>> BuildSampleWorkflow()
	//{
	//	const string currentStepType = "Scan";
	//	const string workflowTypeCode = "ReceiveGoods";
	//	return new Workflow<Dictionary<string, object>>
	//	{
	//		Code = "123",
	//		TypeCode = workflowTypeCode,
	//		Name = "Приемка по накладной",
	//		Description = "Приемка по каждой строчки накладной",
	//		IsActive = true,
	//		Data = new WorkflowDataCollection(),
	//		CurrentStepType = currentStepType,
	//		Steps =
	//		[
	//			new WorkflowStep
	//			{
	//				Type = currentStepType,
	//				Name = "Шаг сканирования",
	//				Description = "Сканирование товара",
	//				Order = 1,
	//				Actions = []
	//			},
	//			new WorkflowStep
	//			{
	//				Type = "Verify",
	//				Name = "Шаг проверки",
	//				Description = "Проверка количества",
	//				Order = 2,
	//				Actions = []
	//			},
	//			new WorkflowStep
	//			{
	//				Type = "Accept",
	//				Name = "Шаг приемки",
	//				Description = "Подтверждение приемки",
	//				Order = 3,
	//				Actions = []
	//			}
	//		]
	//	};
	//}

	//private sealed class WorkflowDataCollection : IWorkflowDataCollection<Dictionary<string, object>>
	//{
	//	public string Name { get; set; } = "Data";
	//	public string Description { get; set; } = "Workflow data";
	//	public ICollection<Dictionary<string, object>> Collection { get; set; } = new List<Dictionary<string, object>>();
	//	public WorkflowDataCollection()
	//	{
	//		Collection.Add(new Dictionary<string, object>
	//		{
	//			["Id"] = 0,
	//			["Barcode"] = "12345678",
	//			["Quantity"] = "1",
	//		});
	//		Collection.Add(new Dictionary<string, object>
	//		{
	//			["Id"] = 1,
	//			["Barcode"] = "87654321",
	//			["Quantity"] = "2",
	//		});
	//
	//	}
	//}

	/// <summary>
	/// Example of creating JsonElement with sample data for CurrentStep.Data field.
	/// This demonstrates how to populate Data with arbitrary JSON structures including arrays and nested objects.
	/// </summary>
	//private static JsonElement CreateSampleData()
	//{
	//	var sampleData = new
	//	{
	//		invoiceNumber = "INV-2024-001",
	//		date = "2024-01-15",
	//		supplier = new
	//		{
	//			name = "ООО Поставщик",
	//			inn = "1234567890"
	//		},
	//		items = new[]
	//		{
	//			new
	//			{
	//				itemCode = "PROD-001",
	//				name = "Товар 1",
	//				quantity = 10,
	//				unit = "шт",
	//				price = 1500.50m,
	//				tags = new[] { "urgent", "fragile" }
	//			},
	//			new
	//			{
	//				itemCode = "PROD-002",
	//				name = "Товар 2",
	//				quantity = 5,
	//				unit = "шт",
	//				price = 2300.00m,
	//				tags = new[] { "standard" }
	//			}
	//		},
	//		metadata = new
	//		{
	//			source = "warehouse",
	//			priority = "high",
	//			notes = new[]
	//			{
	//				"Требуется проверка качества",
	//				"Срочная доставка"
	//			}
	//		}
	//	};
	//
	//	var jsonString = JsonSerializer.Serialize(sampleData);
	//	using var document = JsonDocument.Parse(jsonString);
	//	return document.RootElement.Clone();
	//}

