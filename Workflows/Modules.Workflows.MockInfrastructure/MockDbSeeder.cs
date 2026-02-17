using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Serializers;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.MockInfrastructure;

public static class MockDbSeeder
{
	public static void Seed(WorkflowsDbContext context)
	{
		SeedMultipleWorkflows(context);
	}
	//public static void SeedWorkflows(WorkflowsDbContext context)
	//{
	//	var workflowId = Guid.NewGuid();
	//	var workflow = new Workflow<IDataItem>
	//	{
	//		Id = workflowId,
	//		Code = "TestWorkflow",
	//		Name = "Test Workflow",
	//		Description = "A test workflow for demonstration purposes.",
	//		IsActive = true,
	//		CurrentStepType = "Scan",
	//		TypeCode = "TEST",
	//		CurrentOrder = 0,
	//		Data = new WorkflowDataItemsCollection<IDataItem> { Name = "Items", Description = "Empty Descr", Collection = SeedDataItems(context).Cast<IDataItem>().ToList() },
	//		WorkflowDataItems = SeedDataItems(context, workflowId),
	//		//DataGuids = SeedDataItems(context),
	//		Steps = SeedSteps(workflowId),
	//	};
	//	context.Workflows.Add(workflow);
	//	context.SaveChanges();
	//}

	//private static List<WorkflowStep> SeedSteps(Guid workflowId)
	//{
	//
	//	var step1Id = Guid.NewGuid();
	//	var step2Id = Guid.NewGuid();
	//	return new List<Domain.Entities.WorkflowStep>
	//		{
	//			new WorkflowStep
	//			{
	//				Id = Guid.NewGuid(),
	//				WorkflowId = workflowId,
	//				 Type = "Scan",
	//				 Description = "Scanning the items",
	//				 Name = "Scan",
	//				 Order = 0,
	//				Actions = new List<WorkflowStepAction>
	//				{
	//					new WorkflowStepAction
	//					{
	//					  Id = Guid.NewGuid(),
	//					  StepId = step1Id,
	//					  Name = "SendPackage",
	//					  Type = "SendPackage",
	//					  HttpMethod = "POST",
	//					  Description = "Send scanned barcodes with quantities",
	//					  Endpoint = "SendScannedBarcodes",
	//					  RouteParams = new Dictionary<string, object> {["subpoint"] = "data"},
	//
	//					},
	//					new WorkflowStepAction
	//					{
	//						Id = Guid.NewGuid(),
	//						StepId = step1Id,
	//						Name = "GoToNextStep",
	//						Type = "GoToNextStep",
	//						HttpMethod = "PATCH",
	//						Description = "Go to next step",
	//						Endpoint = "WorkflowNextStep",
	//						RouteParams = [],
	//					}
	//				}
	//			},
	//			new WorkflowStep
	//			{
	//				Id = Guid.NewGuid(),
	//				WorkflowId = workflowId,
	//				Name = "CheckoutName",
	//				Description = "Checking the scanned items",
	//				Order = 1,
	//				Type = "Checkout",
	//				Actions = new List<WorkflowStepAction>
	//				{
	//						new WorkflowStepAction
	//					{
	//						Id = Guid.NewGuid(),
	//						StepId = step2Id,
	//						Name = "GoToNextStepName",
	//						Type = "GoToNextStep",
	//						HttpMethod = "PATCH",
	//						Description = "Go to next step",
	//						Endpoint = "WorkflowNextStep",
	//						RouteParams = [],
	//					},
	//						new WorkflowStepAction
	//					{
	//						Id = Guid.NewGuid(),
	//						StepId = step2Id,
	//						Name = "GoToPreviousStep",
	//						Type = "GoToPreviousStep",
	//						HttpMethod = "PATCH",
	//						Description = "Go to Previous step",
	//						Endpoint = "WorkflowPrevStep",
	//						RouteParams = [],
	//					}
	//				}
	//			}
	//	};
	//}
//	public static List<DataItemInventory> SeedDataItems(WorkflowsDbContext context)
//	{
//		var items = SeedItems();
//		context.WorkflowDataItemInventory.AddRange(items);
//		context.WorkflowDataLinks.AddRange(items.Select(x => new WorkflowDataLink { DataId = x.Id, DataType = "Item" }));
//		//context.SaveChanges();
//		//return items.Select(x => x.Id).ToList();
//		return items;
//	}
//	public static List<WorkflowDataItem> SeedDataItems(WorkflowsDbContext context, Guid workflowId)
//	{
//		var items = SeedItems();
//
//		var workflowDataItems = items.Select(item => new WorkflowDataItem
//		{
//			Id = Guid.NewGuid(),
//			WorkflowId = workflowId,
//			Name = item.Name,
//			Description = item.Description,
//			Code = item.ItemCode,
//			Quantity = item.Quantity,
//			Discriminator = "inventory",
//			JsonData = JsonSerializer.Serialize(item, DataItemSerializer.DataItemSeializerOptions),
//		}).ToList();
//
//		context.WorkflowDataItems.AddRange(workflowDataItems);
//		//context.SaveChanges();
//
//		return workflowDataItems;
//	}
//	private static List<DataItemInventory> SeedItems()
//	{
//		return new List<DataItemInventory>
//		{
//			new DataItemInventory
//			{
//				Id = Guid.NewGuid(),
//				Name = "Товар 1",
//				Description = "",
//				ItemCode = "PROD-001",
//				Quantity = 10,
//				Units = "шт",
//				Price = 1500.50m,
//#pragma warning disable CA1861 //для моков пойдет
//				Tags = new[] { "urgent", "fragile" },
//#pragma warning restore CA1861
//			},
//			new DataItemInventory
//			{
//				Id = Guid.NewGuid(),
//				Name = "Товар 2",
//				Description = "",
//				ItemCode = "PROD-002",
//				Quantity = 5,
//				Units = "шт",
//				Price = 2300.50m,
//#pragma warning disable CA1861 //для моков пойдет
//				Tags = new[] { "standard" },
//#pragma warning restore CA1861
//			}
//		};
//	}
	private static void SeedMultipleWorkflows(WorkflowsDbContext context)
	{
		var workflows = new[]
		{
			CreateInboundWorkflow(context),
			CreateOutboundWorkflow(context),
			CreateInventoryCountWorkflow(context),
			CreateReturnsWorkflow(context)
		};

		context.Workflows.AddRange(workflows);
		context.SaveChanges();
	}

	private static Workflow<IDataItem> CreateInboundWorkflow(WorkflowsDbContext context)
	{
		var id = Guid.NewGuid();
		return new Workflow<IDataItem>
		{
			Id = id,
			Code = "INBOUND-01",
			Name = "Приёмка товара",
			Description = "Приёмка и первичная проверка поступившего товара",
			IsActive = false,
			CurrentStepType = "Scan",
			TypeCode = "INBOUND",
			CurrentStepNumber = 1,
			Data = new WorkflowDataItemsCollection<IDataItem> { Name = "Поступление", Description = "", Collection = SeedInboundItems().Cast<IDataItem>().ToList() },
			WorkflowDataItems = SeedWorkflowDataItems(context, id, "inbound"),
			Steps = SeedInboundSteps(id)
		};
	}

	private static Workflow<IDataItem> CreateOutboundWorkflow(WorkflowsDbContext context)
	{
		var id = Guid.NewGuid();
		return new Workflow<IDataItem>
		{
			Id = id,
			Code = "OUTBOUND-01",
			Name = "Отгрузка / Комплектация",
			Description = "",
			IsActive = true,
			CurrentStepType = "Scan",
			TypeCode = "OUTBOUND",
			CurrentStepNumber = 1,
			Steps = SeedOutboundSteps(id),
			WorkflowDataItems = SeedWorkflowDataItems(context, id, "outbound")
		};
	}

	private static Workflow<IDataItem> CreateInventoryCountWorkflow(WorkflowsDbContext context)
	{
		var id = Guid.NewGuid();
		return new Workflow<IDataItem>
		{
			Id = id,
			Code = "COUNT-2025",
			Name = "Инвентаризация",
			Description = "",
			IsActive = false,
			CurrentStepType = "Verify",
			TypeCode = "INVENTORY",
			CurrentStepNumber = 1,
			Steps = SeedInventorySteps(id),
			WorkflowDataItems = SeedWorkflowDataItems(context, id, "count")
		};
	}

	private static Workflow<IDataItem> CreateReturnsWorkflow(WorkflowsDbContext context)
	{
		var id = Guid.NewGuid();
		return new Workflow<IDataItem>
		{
			Id = id,
			Code = "RETURN-01",
			Name = "Обработка возвратов",
			Description = "",
			IsActive = true,
			CurrentStepType = "Scan",
			TypeCode = "RETURN",
			CurrentStepNumber = 1,
			Steps = SeedReturnSteps(id),
			WorkflowDataItems = SeedWorkflowDataItems(context, id, "return")
		};
	}

	// ────────────────────────────────────────────────
	// Шаги для разных процессов
	// ────────────────────────────────────────────────

	private static List<WorkflowStep> SeedInboundSteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); // Scan
		var s2 = Guid.NewGuid(); // Verify
		var s3 = Guid.NewGuid(); // Accept / Putaway

		return new List<WorkflowStep>
		{
			new() { Id = s1, WorkflowId = workflowId, Type = "Scan", Name = "Сканирование", Order = 0, Description = "Сканируем коробки / паллеты / штуки", Actions = [ SendPackage(s1), NextStep(s1), CancelWorkflow(s1), ] }, //SESZH: пусть в этом воркфлоу его можно будет отменить когда угодно, потом разберемся, когда можно и где
			new() { Id = s2, WorkflowId = workflowId, Type = "Verify",  Name = "Проверка", Order = 1, Description = "Сверка фактического кол-ва с документом", Actions = [ NextStep(s2), PrevStep(s2), CancelWorkflow(s2)] },
			new() { Id = s3, WorkflowId = workflowId, Type = "Accept",  Name = "Принятие на склад", Order = 2, Description = "Подтверждение и размещение", Actions = [ PrevStep(s3), CompleteWorkflow(s3), CancelWorkflow(s3)] }
		};
	}

	private static List<WorkflowStep> SeedOutboundSteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); // Pick / Scan
		var s2 = Guid.NewGuid(); // Pack / Verify
		var s3 = Guid.NewGuid(); // Ship

		return new List<WorkflowStep>
		{
			new() { Id = s1, WorkflowId = workflowId, Type = "Scan",   Name = "Подбор",    Description = "",      Order = 1, Actions = [ SendPackage(s1), NextStep(s1) ] },
			new() { Id = s2, WorkflowId = workflowId, Type = "Verify", Name = "Сверка", Description = "", Order = 2, Actions = [ SendPackage(s2), NextStep(s2), PrevStep(s2) ] },
			new() { Id = s3, WorkflowId = workflowId, Type = "Accept", Name = "Отгрузка",     Description = "",    Order = 3, Actions = [PrevStep(s3), CompleteWorkflow(s3) ] }
		};
	}

	private static List<WorkflowStep> SeedInventorySteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); 
		var s2 = Guid.NewGuid(); 

		return new List<WorkflowStep>
		{
			new() { Id = s1, WorkflowId = workflowId, Type = "Scan",   Name = "Сканирование",   Description = "",      Order = 1, Actions = [ SendPackage(s1), NextStep(s1) ] },
			new() { Id = s2, WorkflowId = workflowId, Type = "Verify", Name = "Сверка расхождений", Description = "", Order = 2, Actions = [PrevStep(s2), CompleteWorkflow(s2) ] }
		};
	}

	private static List<WorkflowStep> SeedReturnSteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); // Scan returned items
		var s2 = Guid.NewGuid(); // Quality check
		var s3 = Guid.NewGuid(); // Accept / Reject

		return new List<WorkflowStep>
		{
			new() { Id = s1, WorkflowId = workflowId, Type = "Scan",   Name = "Скан возврата", Description = "",  Order = 1, Actions = [ SendPackage(s1), NextStep(s1) ] },
			new() { Id = s2, WorkflowId = workflowId, Type = "Verify", Name = "Проверка качества", Description = "", Order = 2, Actions = [ NextStep(s2), PrevStep(s2) ] },
			new() { Id = s3, WorkflowId = workflowId, Type = "Accept", Name = "Приём", Description = "", Order = 3, Actions = [PrevStep(s3), CompleteWorkflow(s3) ] }
		};
	}
	private static WorkflowStepAction SendPackage(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "SendData",
		Type = "SendPackage",
		HttpMethod = "POST",
		Endpoint = "data",
		Description = "Отправка данных шага",
		RouteParams = [],
	};

	private static WorkflowStepAction NextStep(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "Next",
		Type = "GoToNextStep",
		HttpMethod = "PATCH",
		Endpoint = "WorkflowNextStep",
		Description = "",
		RouteParams = [],
	};

	private static WorkflowStepAction PrevStep(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "Prev",
		Type = "GoToPreviousStep",
		HttpMethod = "PATCH",
		Endpoint = "WorkflowPrevStep",
		Description = "Предыдущий шаг",
		RouteParams = [],
	};

	private static WorkflowStepAction CompleteWorkflow(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "Complete",
		Type = "CompleteWorkflow",
		HttpMethod = "PATCH",
		Endpoint = "CompleteWorkflow",
		Description = "Завершение процесса",
		RouteParams = [],
	};
	private static WorkflowStepAction CancelWorkflow(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "Cancel",
		Type = "CancelWorkflow",
		HttpMethod = "PATCH",
		Endpoint = "CancelWorkflow",
		Description = "Отмена процесса",
		RouteParams = [],
	};
	private static List<WorkflowDataItem> SeedWorkflowDataItems(WorkflowsDbContext context, Guid workflowId, string type)
	{
		var baseItems = type switch
		{
			"inbound" => SeedInboundItems(),
			"outbound" => SeedOutboundItems(),
			"count" => SeedCountItems(),
			"return" => SeedReturnItems(),
			_ => SeedInboundItems()
		};

		var wdi = baseItems.Select(i => new WorkflowDataItem
		{
			Id = Guid.NewGuid(),
			WorkflowId = workflowId,
			Name = i.Name,
			Description = "",
			Code = i.ItemCode,
			Quantity = i.Quantity,
			Discriminator = "inventory",
			JsonData = JsonSerializer.Serialize(i, DataItemSerializer.DataItemSeializerOptions)
		}).ToList();

		context.WorkflowDataItems.AddRange(wdi);
		return wdi;
	}
	private static List<DataItemInventory> SeedInboundItems() => [
		new() { Id = Guid.NewGuid(), Name = "Ноутбук Dell XPS", Description = "",     ItemCode = "LAP-DX13", Quantity = 48,  Price = 1890.00m, Units = "шт", Tags = ["electronics", "new"] },
		new() { Id = Guid.NewGuid(), Name = "Монитор 27\" 4K",   Description = "",     ItemCode = "MON-4K27", Quantity = 120, Price = 420.75m,  Units = "шт", Tags = ["display"] },
		new() { Id = Guid.NewGuid(), Name = "Клавиатура механическая", Description = "", ItemCode = "KEY-MK02", Quantity = 200, Price = 89.90m,  Units = "шт" }
	];

	private static List<DataItemInventory> SeedOutboundItems() => [
		new() { Id = Guid.NewGuid(), Name = "Смартфон Galaxy S24",   Description = "", ItemCode = "PHN-S24",  Quantity = 15,  Price = 799.00m, Units = "шт", Tags = ["mobile", "hot"] },
		new() { Id = Guid.NewGuid(), Name = "Беспроводные наушники", Description = "", ItemCode = "EAR-BW01", Quantity = 65,  Price = 149.00m, Units = "шт" },
		new() { Id = Guid.NewGuid(), Name = "Зарядка 65W GaN",       Description = "", ItemCode = "CHR-65W",  Quantity = 80,  Price = 39.50m,  Units = "шт" }
	];

	private static List<DataItemInventory> SeedCountItems() => [
		new() { Id = Guid.NewGuid(), Name = "Кофе в зернах 1 кг",  Description = "",   ItemCode = "COF-001",  Quantity = 0,   Price = 22.80m,  Units = "упак" },
		new() { Id = Guid.NewGuid(), Name = "Чай черный 200 г",    Description = "",   ItemCode = "TEA-BLK",  Quantity = 0,   Price = 8.40m,   Units = "пач" },
		new() { Id = Guid.NewGuid(), Name = "Сахар-песок 5 кг",   Description = "",    ItemCode = "SUG-005",  Quantity = 0,   Price = 3.99m,   Units = "меш" }
	];

	private static List<DataItemInventory> SeedReturnItems() => [
		new() { Id = Guid.NewGuid(), Name = "Пылесос робот",        Description = "",  ItemCode = "VAC-ROB2", Quantity = 7,   Price = 320.00m, Units = "шт", Tags = ["return"] },
		new() { Id = Guid.NewGuid(), Name = "Фен профессиональный", Description = "",  ItemCode = "DRY-PRO",  Quantity = 4,   Price = 115.00m, Units = "шт", Tags = ["used"] }
	];
}

