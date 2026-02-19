using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Domain.Serializers;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.MockInfrastructure;

public static class MockDbSeeder
{
	public static void Seed(WorkflowsDbContext context)
	{
		SeedMultipleWorkflows(context);
	}

	private static void SeedMultipleWorkflows(WorkflowsDbContext context)
	{
		var workflowTypes = SeedWorkflowTypes(context);
		var workflows = new List<Workflow>();

		// По несколько воркфлоу на каждый тип
		workflows.AddRange(CreateInboundWorkflows(workflowTypes["INBOUND"]));
		//workflows.AddRange(CreateOutboundWorkflows(workflowTypes["OUTBOUND"]));
		//workflows.AddRange(CreateInventoryCountWorkflows(workflowTypes["INVENTORY"]));
		workflows.AddRange(CreateReturnsWorkflows(workflowTypes["RETURN"]));

		context.Workflows.AddRange(workflows);
		context.SaveChanges();

		SaveInvoiceHeaders(context, workflows);
	}

	private static void SaveInvoiceHeaders(WorkflowsDbContext context, List<Workflow> workflows)
	{
		foreach (var w in workflows)
		{
			if (w.Data is IWorkflowDataCollection<InvoiceHeader> headerCollection)
			{
				context.InvoiceHeaders.AddRange(headerCollection.Collection);
			}
		}
		context.SaveChanges();
	}

	private static readonly string[] InboundNames = ["Приёмка товара", "Приёмка поставки", "Входящая приёмка"];
	//private static readonly string[] OutboundNames = ["Отгрузка / Комплектация", "Комплектация заказа", "Отгрузка со склада"];
	//private static readonly string[] InventoryNames = ["Инвентаризация", "Пересчёт остатков", "Инвентаризация зоны"];
	private static readonly string[] ReturnNames = ["Обработка возвратов", "Приём возврата", "Возврат от клиента"];

	private static List<Workflow> CreateInboundWorkflows(WorkflowType workflowType)
	{
		var list = new List<Workflow>();
		for (var i = 1; i <= 3; i++)
		{
			var id = Guid.NewGuid();
			list.Add(new Workflow
			{
				Id = id,
				Code = $"INBOUND-{i:D2}",
				Name = InboundNames[i - 1],
				Description = "Приёмка и первичная проверка поступившего товара",
				IsActive = true,
				CurrentStepType = "Scan",
				Type = workflowType,
				CurrentStepNumber = 1,
				Data = new DefaultWorkflowDataCollection<InvoiceHeader>
				{
					Name = "Приёмка",
					Description = "",
					Collection = new List<InvoiceHeader>
					{
						new InvoiceHeader
						{
							WorkflowId = id,
							Counterparty = $"Поставщик {i}",
							Contract = $"INV-{i:D4}",
							InvoiceId = Guid.NewGuid().ToString(),
						}
					}
				},
				//Data = new WorkflowDataItemsCollection<IDataItem> { Name = "Поступление", Description = "", Collection = SeedInboundItems().Cast<IDataItem>().ToList() },
				//WorkflowDataItems = SeedWorkflowDataItems(context, id, "inbound"),
				Steps = SeedInboundSteps(id)
			});
		}
		return list;
	}

	//private static List<Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>> CreateOutboundWorkflows(WorkflowType workflowType)
	//{
	//	var list = new List<Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>>();
	//	for (var i = 1; i <= 3; i++)
	//	{
	//		var id = Guid.NewGuid();
	//		list.Add(new Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>
	//		{
	//			Id = id,
	//			Code = $"OUTBOUND-{i:D2}",
	//			Name = OutboundNames[i - 1],
	//			Description = "",
	//			IsActive = true,
	//			CurrentStepType = "Scan",
	//			Type = workflowType,
	//			CurrentStepNumber = 1,
	//			Steps = SeedOutboundSteps(id),
	//			Data = new DefaultWorkflowData<DefaultWorkflowData<InvoiceHeader>>
	//			{
	//				Name = "Отгрузка",
	//				Description = "",
	//				Data = new DefaultWorkflowData<InvoiceHeader>
	//				{
	//					Name = $"{OutboundNames[i - 1]}.TempName",
	//					Description = string.Empty,
	//					Data = new InvoiceHeader
	//					{
	//						InvoiceId = Guid.NewGuid().ToString(),
	//						Contract = $"OUT-{i:D4}",
	//						Counterparty = $"Отправитель {i}"
	//					}
	//				}
	//			},
	//			//WorkflowDataItems = SeedWorkflowDataItems(context, id, "outbound")
	//		});
	//	}
	//	return list;
	//}

	//private static List<Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>> CreateInventoryCountWorkflows(WorkflowType workflowType)
	//{
	//	var list = new List<Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>>();
	//	for (var i = 1; i <= 2; i++)
	//	{
	//		var id = Guid.NewGuid();
	//		list.Add(new Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>
	//		{
	//			Id = id,
	//			Code = $"COUNT-2025-{i:D2}",
	//			Name = InventoryNames[i - 1],
	//			Description = "",
	//			IsActive = true,
	//			CurrentStepType = "Scan",
	//			Type = workflowType,
	//			CurrentStepNumber = 1,
	//			Steps = SeedInventorySteps(id),
	//			Data = new DefaultWorkflowData<DefaultWorkflowData<InvoiceHeader>>
	//			{
	//				Name = "Инвентаризация",
	//				Description = "",
	//				Data = new DefaultWorkflowData<InvoiceHeader>
	//				{
	//					Name = $"{InventoryNames[i - 1]}.TempName",
	//					Description = string.Empty,
	//					Data = new InvoiceHeader
	//					{
	//						InvoiceId = Guid.NewGuid().ToString(),
	//						Contract = $"INV-{i:D4}",
	//						Counterparty = $"Склад {i}"
	//					}
	//				}
	//			},
	//			//WorkflowDataItems = SeedWorkflowDataItems(context, id, "count")
	//		});
	//	}
	//	var id3 = Guid.NewGuid();
	//	list.Add(new Workflow<DefaultWorkflowData<InvoiceHeader>, Invoice>
	//	{
	//		Id = id3,
	//		Code = $"COUNT-2025-{3:D2}",
	//		Name = InventoryNames[2],
	//		Description = "",
	//		IsActive = false,
	//		CurrentStepType = "Scan",
	//		Type = workflowType,
	//		CurrentStepNumber = 1,
	//		Steps = SeedInventorySteps(id3),
	//		Data = new DefaultWorkflowData<DefaultWorkflowData<InvoiceHeader>>
	//		{
	//			Name = "Инвентаризация",
	//			Description = "",
	//			Data = new DefaultWorkflowData<InvoiceHeader>
	//			{
	//				Name = $"{InventoryNames[2]}.TempName",
	//				Description = string.Empty,
	//				Data = new InvoiceHeader
	//				{
	//					InvoiceId = Guid.NewGuid().ToString(),
	//					Contract = $"INV-{3:D4}",
	//					Counterparty = $"Склад {3}"
	//				}
	//			}
	//		}
	//		//WorkflowDataItems = SeedWorkflowDataItems(context, id3, "count")
	//	});
	//	return list;
	//}

	private static List<Workflow> CreateReturnsWorkflows(WorkflowType workflowType)
	{
		var list = new List<Workflow>();
		for (var i = 1; i <= 3; i++)
		{
			var id = Guid.NewGuid();
			list.Add(new Workflow
			{
				Id = id,
				Code = $"RETURN-{i:D2}",
				Name = ReturnNames[i - 1],
				Description = "",
				IsActive = true,
				CurrentStepType = "Scan",
				Type = workflowType,
				CurrentStepNumber = 1,
				Steps = SeedReturnSteps(id),
				Data = new DefaultWorkflowDataCollection<InvoiceHeader>
				{
					Name = "Возврат",
					Description = "",
					Collection = new List<InvoiceHeader>
					{
						new InvoiceHeader
						{
							WorkflowId = id,
							InvoiceId = Guid.NewGuid().ToString(),
							Contract = $"INV-{i:D4}",
							Counterparty = $"Поставщик {i}",
						}
					}
				}
			});
		}
			//WorkflowDataItems = SeedWorkflowDataItems(context, id, "return")
	return list;
	}

	// ────────────────────────────────────────────────
	// Шаги для разных процессов
	// ────────────────────────────────────────────────

	private static List<WorkflowStep> SeedInboundSteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); // Scan
		var s2 = Guid.NewGuid(); // Verify
		var s3 = Guid.NewGuid(); // Accept / Putaway
		var scanData = SeedInboundItems();

		return new List<WorkflowStep>
		{
			new()
			{
				Id = s1,
				WorkflowId = workflowId,
				StepCode = "Scan-01",
				Type = "Scan",
				Name = "Сканирование",
				Order = 0,
				Description = "Сканируем коробки / паллеты / штуки",
				Actions =
				[ SendPackage(s1), NextStep(s1), CancelWorkflow(s1), ],
				Data = new DefaultWorkflowDataCollection<Invoice>
				{
					Name = "Сканирование",
					Description = "Сканируем коробки / паллеты / штуки",
					Collection = new List<Invoice>
					{
						new Invoice
						{
							WorkflowId = workflowId,
							StepId = s1,
							InvoiceId = "SESZH: должен быть, как у родительского воркфлоу",
							Counterparty = "Аналогично",
							Contract = "Аналогично",
							Name = "Аналогично",
							Date = DateTime.UtcNow.AddDays(-1),
							Lines = scanData.Select(l => new InvoiceLine
							{
								ProductName = l.Name,
								Quantity = l.Quantity,
								Units = l.Units,
								Barcode = l.ItemCode
							}).ToList()
						}
					}
				}
			}, //SESZH: пусть в этом воркфлоу его можно будет отменить когда угодно, потом разберемся, когда можно и где
			new() 
			{ 
				Id = s2, 
				WorkflowId = workflowId, 
				StepCode = "Verify-02", 
				Type = "Verify",  
				Name = "Проверка", 
				Order = 1, 
				Description = "Сверка фактического кол-ва с документом", 
				Actions = [ NextStep(s2), PrevStep(s2), CancelWorkflow(s2)], 
				Data = new DefaultWorkflowDataCollection<Invoice> 
				{ 
					Name = "Проверка",
					Description = "Сверка фактического кол-ва с документом",
						Collection = new List<Invoice>(),
				}
			},
			new() 
			{ 
				Id = s3, 
				WorkflowId = workflowId, 
				StepCode = "Accept-03", 
				Type = "Accept",  
				Name = "Принятие на склад", 
				Order = 2, 
				Description = "Подтверждение и размещение", 
				Actions = [ PrevStep(s3), CompleteWorkflow(s3), CancelWorkflow(s3)], 
				Data = new DefaultWorkflowDataCollection<Invoice> 
				{ 
					Name = "Принятие на склад",
					Description = "Подтверждение и размещение",
					Collection = new List<Invoice>(),
				}
			}
		};
	}

	//private static List<WorkflowStep<Invoice>> SeedOutboundSteps(Guid workflowId)
	//{
	//	var s1 = Guid.NewGuid(); // Pick / Scan
	//	var s2 = Guid.NewGuid(); // Pack / Verify
	//	var s3 = Guid.NewGuid(); // Ship
	//
	//	return new List<WorkflowStep<Invoice>>
	//	{
	//		new() 
	//		{ 
	//			Id = s1, 
	//			WorkflowId = workflowId, 
	//			StepCode = "Scan-01", 
	//			Type = "Scan",   
	//			Name = "Подбор",    
	//			Description = "",      
	//			Order = 1, 
	//			Actions = [ SendPackage(s1), NextStep(s1) ], 
	//			Data = new DefaultWorkflowDataCollection<Invoice> 
	//			{ 
	//				Name = "Подбор", 
	//				Description = "", 
	//				Collection = new List<Invoice>()
	//			}
	//		},
	//		new() 
	//		{ 
	//			Id = s2, 
	//			WorkflowId = workflowId, 
	//			StepCode = "Verify-02", 
	//			Type = "Verify", 
	//			Name = "Сверка", 
	//			Description = "", Order = 2, 
	//			Actions = [ SendPackage(s2), NextStep(s2), PrevStep(s2) ], 
	//			Data = new DefaultWorkflowDataCollection<Invoice> 
	//			{ Name = "Сверка", Description = "", Collection = new List<Invoice>() } 
	//			
	//		},
	//		new() 
	//		{ 
	//			Id = s3, 
	//			WorkflowId = workflowId, 
	//			StepCode = "Accept-03", 
	//			Type = "Accept", 
	//			Name = "Отгрузка",     
	//			Description = "",    
	//			Order = 3, 
	//			Actions = [PrevStep(s3), CompleteWorkflow(s3) ], 
	//			Data = new DefaultWorkflowDataCollection<Invoice> 
	//			{ Name = "Отгрузка", Description = "", Collection = new List<Invoice>() } 
	//		}
	//	};
	//}

    //	private static List<WorkflowStep<Invoice>> SeedInventorySteps(Guid workflowId)
    //	{
    //		var s1 = Guid.NewGuid();
    //		var s2 = Guid.NewGuid();
    //
	//	return new List<WorkflowStep<Invoice>>
	//	{
	//		new() { Id = s1, WorkflowId = workflowId, StepCode = "Scan-01", Type = "Scan",   Name = "Сканирование",   Description = "",      Order = 1, Actions = [ SendPackage(s1), NextStep(s1) ], Data = new DefaultWorkflowDataCollection<Invoice> { Name = "Сканирование", Description = "", Collection = new List<Invoice>() } },
	//		new() { Id = s2, WorkflowId = workflowId, StepCode = "Verify-02", Type = "Verify", Name = "Сверка расхождений", Description = "", Order = 2, Actions = [PrevStep(s2), CompleteWorkflow(s2) ], Data = new DefaultWorkflowDataCollection<Invoice> { Name = "Сверка расхождений", Description = "", Collection = new List<Invoice>() } }
	//	};
	//}

	private static List<WorkflowStep> SeedReturnSteps(Guid workflowId)
	{
		var s1 = Guid.NewGuid(); // Scan returned items
		var s2 = Guid.NewGuid(); // Quality check
		var s3 = Guid.NewGuid(); // Accept / Reject
		var scanData = SeedReturnItems();

		return new List<WorkflowStep>
		{
			new() 
			{ 
				Id = s1, 
				WorkflowId = workflowId, 
				StepCode = "Scan-01", 
				Type = "Scan",   
				Name = "Скан возврата", 
				Description = "",  
				Order = 1, 
				Actions = [ SendPackage(s1), NextStep(s1) ], 
				Data = new DefaultWorkflowDataCollection<Invoice> 
				{ 
					Name = "Скан возврата", 
					Description = "", 
					Collection = new List<Invoice>
					{
						new Invoice
						{
							WorkflowId = workflowId,
							StepId = s1,
							InvoiceId = "SESZH: должен быть, как у родительского воркфлоу",
							Counterparty = "Аналогично",
							Contract = "Аналогично",
							Name = "Аналогично",
							Date = DateTime.UtcNow.AddDays(-1),
							Lines = scanData.Select(l => new InvoiceLine
							{
								ProductName = l.Name,
								Quantity = l.Quantity,
								Units = l.Units,
								Barcode = l.ItemCode
							}).ToList(),
						}
					}
				}
			},
			new() {
				Id = s2,
				WorkflowId = workflowId,
				StepCode = "Verify-02",
				Type = "Verify",
				Name = "Проверка качества",
				Description = "",
				Order = 2,
				Actions = [ NextStep(s2), PrevStep(s2) ],
				Data = new DefaultWorkflowDataCollection<Invoice>
				{
					Name = "Проверка качества",
					Description = "",
					Collection = new List<Invoice>(),
				}
			},
			new() 
			{ 
				Id = s3, 
				WorkflowId = workflowId, 
				StepCode = "Accept-03", 
				Type = "Accept", 
				Name = "Приём", 
				Description = "", 
				Order = 3, 
				Actions = [PrevStep(s3), CompleteWorkflow(s3) ], 
				Data = new DefaultWorkflowDataCollection<Invoice> 
				{ 
					Name = "Приём", 
					Description = "", 
					Collection = new List<Invoice>(),
				}
			}
		};
	}
	private static WorkflowStepAction SendPackage(Guid stepId) => new()
	{
		Id = Guid.NewGuid(),
		StepId = stepId,
		Name = "SendData",
		Type = "SendPackage",
		HttpMethod = "POST",
		Endpoint = "SendScannedBarcodes",
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
	private static List<DataItemInventory> SeedInboundItems() => [
		new() { Id = Guid.NewGuid(), Name = "Ноутбук Dell XPS", Description = "",     ItemCode = "LAP-DX13", Quantity = 48,  Price = 1890.00m, Units = "шт", Tags = ["electronics", "new"] },
		new() { Id = Guid.NewGuid(), Name = "Монитор 27\" 4K",   Description = "",     ItemCode = "MON-4K27", Quantity = 120, Price = 420.75m,  Units = "шт", Tags = ["display"] },
		new() { Id = Guid.NewGuid(), Name = "Клавиатура механическая", Description = "", ItemCode = "KEY-MK02", Quantity = 200, Price = 89.90m,  Units = "шт" }
	];

	//private static List<DataItemInventory> SeedOutboundItems() => [
	//	new() { Id = Guid.NewGuid(), Name = "Смартфон Galaxy S24",   Description = "", ItemCode = "PHN-S24",  Quantity = 15,  Price = 799.00m, Units = "шт", Tags = ["mobile", "hot"] },
	//	new() { Id = Guid.NewGuid(), Name = "Беспроводные наушники", Description = "", ItemCode = "EAR-BW01", Quantity = 65,  Price = 149.00m, Units = "шт" },
	//	new() { Id = Guid.NewGuid(), Name = "Зарядка 65W GaN",       Description = "", ItemCode = "CHR-65W",  Quantity = 80,  Price = 39.50m,  Units = "шт" }
	//];
	//
	//private static List<DataItemInventory> SeedCountItems() => [
	//	new() { Id = Guid.NewGuid(), Name = "Кофе в зернах 1 кг",  Description = "",   ItemCode = "COF-001",  Quantity = 0,   Price = 22.80m,  Units = "упак" },
	//	new() { Id = Guid.NewGuid(), Name = "Чай черный 200 г",    Description = "",   ItemCode = "TEA-BLK",  Quantity = 0,   Price = 8.40m,   Units = "пач" },
	//	new() { Id = Guid.NewGuid(), Name = "Сахар-песок 5 кг",   Description = "",    ItemCode = "SUG-005",  Quantity = 0,   Price = 3.99m,   Units = "меш" }
	//];

	private static List<DataItemInventory> SeedReturnItems() => [
		new() { Id = Guid.NewGuid(), Name = "Пылесос робот",        Description = "",  ItemCode = "VAC-ROB2", Quantity = 7,   Price = 320.00m, Units = "шт", Tags = ["return"] },
		new() { Id = Guid.NewGuid(), Name = "Фен профессиональный", Description = "",  ItemCode = "DRY-PRO",  Quantity = 4,   Price = 115.00m, Units = "шт", Tags = ["used"] }
	];
	private static Dictionary<string, WorkflowType> SeedWorkflowTypes(WorkflowsDbContext context)
{
    var types = new[]
    {
        new WorkflowType("INBOUND", "Приёмка")   { Id = Guid.NewGuid() },
        new WorkflowType("OUTBOUND", "Отгрузка") { Id = Guid.NewGuid() },
        new WorkflowType("INVENTORY", "Инвентаризация") { Id = Guid.NewGuid() },
        new WorkflowType("RETURN", "Возвраты")   { Id = Guid.NewGuid() }
    };

    context.WorkflowTypes.AddRange(types);
    context.SaveChanges();

    return types.ToDictionary(t => t.Code, t => t);
}
}

