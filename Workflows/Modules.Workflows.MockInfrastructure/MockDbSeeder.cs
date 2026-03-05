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

	}



	private static readonly string[] InboundNames = ["Приёмка товара", "Приёмка поставки", "Входящая приёмка"];
	//private static readonly string[] OutboundNames = ["Отгрузка / Комплектация", "Комплектация заказа", "Отгрузка со склада"];
	//private static readonly string[] InventoryNames = ["Инвентаризация", "Пересчёт остатков", "Инвентаризация зоны"];
	private static readonly string[] ReturnNames = ["Обработка возвратов", "Приём возврата", "Возврат от клиента"];

	private static List<Workflow> CreateInboundWorkflows(WorkflowType workflowType)
	{
		var list = new List<Workflow>();

		var id1 = Guid.Parse("0bdafc87-c7f2-439c-a454-f326d52b590c");
		var id2 = Guid.Parse("a78b48d0-dab9-42a0-b5fb-4fa76b8674cc");
		var id3 = Guid.Parse("cae4fee6-a0a3-495c-8592-793b829c0923");

		var w1s1 = Guid.Parse("68b71d26-9a0c-423e-b4df-beb710f217f7"); // Scan
		var w1s2 = Guid.Parse("e8d8e01d-e526-46ea-92b9-642c0cd9dcc9"); // Verify
		var w1s3 = Guid.Parse("56e23c91-5017-4c2f-b949-8e9bd17beb88"); // Accept / Putaway

		var w2s1 = Guid.Parse("c2dbe63e-7ce3-40f5-813b-c7adefa45c87"); // Scan
		var w2s2 = Guid.Parse("ad5d407f-7ba4-4400-8558-5708a14190fe"); // Verify
		var w2s3 = Guid.Parse("f2a88d1c-5eb3-49b4-aae3-30f0d6cb1e39"); // Accept / Putaway

		var w3s1 = Guid.Parse("ae2ecb08-44a8-4ab4-b719-791eddf7d012"); // Scan
		var w3s2 = Guid.Parse("8803a1fa-fa7f-4537-981d-5bbc52514eb9"); // Verify
		var w3s3 = Guid.Parse("1ff1b2a5-b33f-498c-a68c-1c2d6549611c"); // Accept / Putaway


			list.Add(new Workflow
			{ 
				Id = id1,
				Code = "INBOUND-01",
				Name = InboundNames[0],
				Description = "Приёмка и первичная проверка поступившего товара",
				IsActive = true,
				CurrentStepType = WorkflowStepType.Scan,
				Type = workflowType,
				CurrentStepNumber = 1,
				Data = null,
				Steps = SeedInboundSteps(id1, w1s1, w1s2, w1s3),
			});
			list.Add(new Workflow
			{
				Id = id2,
				Code = "INBOUND-02",
				Name = InboundNames[1],
				Description = "Приёмка и первичная проверка поступившего товара",
				IsActive = true,
				CurrentStepType = WorkflowStepType.Scan,
				Type = workflowType,
				CurrentStepNumber = 1,
				Data = null,
				Steps = SeedInboundSteps(id2, w2s1, w2s2, w2s3),
			});
			list.Add(new Workflow
			{
				Id = id3,
				Code = "INBOUND-03",
				Name = InboundNames[2],
				Description = "Приёмка и первичная проверка поступившего товара",
				IsActive = false,
				CurrentStepType = WorkflowStepType.Scan,
				Type = workflowType,
				CurrentStepNumber = 1,
				Data = null,
			Steps = SeedInboundSteps(id3, w3s1, w3s2, w3s3),
		});
		return list;
	}

	private static List<Workflow> CreateReturnsWorkflows(WorkflowType workflowType)
	{
		var list = new List<Workflow>();
		var id1 = Guid.Parse("a0dba775-8829-41dd-ba19-0bcd19627ba4");
		var id2 = Guid.Parse("7dd025bc-be31-442f-942b-c6b547c1898c");
		var id3 = Guid.Parse("80d5fd23-eba8-40f2-bbd6-538959f13d19");

		var w1s1 = Guid.Parse("9f8faaee-bdec-4607-8c86-5a6d6fb51096"); // Scan returned items
		var w1s2 = Guid.Parse("6058c88f-78ff-44bb-a733-8d94426c482a"); // Quality check
		var w1s3 = Guid.Parse("0101aca3-8170-44b2-99ab-bb2a5a00eeb3"); // Accept / Reject

		var w2s1 = Guid.Parse("31429522-d440-490a-9100-37386c23643d"); // Scan returned items
		var w2s2 = Guid.Parse("d7228348-4112-4745-8940-8a6747526763"); // Quality check
		var w2s3 = Guid.Parse("e36f766c-0435-4310-a172-166641c0542f"); // Accept / Reject

		var w3s1 = Guid.Parse("1b1d30a2-8752-435c-95b3-893392343733"); // Scan returned items
		var w3s2 = Guid.Parse("46705528-8065-4665-a76e-90a683953636"); // Quality check
		var w3s3 = Guid.Parse("7656445e-9131-403e-9d66-042403f8657a"); // Accept / Reject

		
		list.Add(new Workflow
		{
			Id = id1,
			Code = "RETURN-01",
			Name = ReturnNames[0],
				Description = "",
				IsActive = true,
				CurrentStepType = WorkflowStepType.Scan,
				Type = workflowType,
				CurrentStepNumber = 1,
				Steps = SeedReturnSteps(id1, w1s1, w1s2, w1s3),
				Data = null
				});
		list.Add(new Workflow
		{
			Id = id2,
			Code = "RETURN-02",
			Name = ReturnNames[1],
			Description = "",
			IsActive = true,
			CurrentStepType = WorkflowStepType.Scan,
			Type = workflowType,
			CurrentStepNumber = 1,
			Steps = SeedReturnSteps(id2, w2s1, w2s2, w2s3),
			Data = null
		});
		list.Add(new Workflow
		{
			Id = id3,
			Code = "RETURN-03",
			Name = ReturnNames[2],
			Description = "",
			IsActive = true,
			CurrentStepType = WorkflowStepType.Scan,
			Type = workflowType,
			CurrentStepNumber = 1,
			Steps = SeedReturnSteps(id3, w3s1, w3s2, w3s3),
			Data = null
		});
		return list;
	}

	// ────────────────────────────────────────────────
	// Шаги для разных процессов
	// ────────────────────────────────────────────────

	private static List<WorkflowStep> SeedInboundSteps(Guid workflowId, Guid s1, Guid s2, Guid s3)
	{
		return new List<WorkflowStep>
		{
			new()
			{
				Id = s1,
				WorkflowId = workflowId,
				StepCode = "Scan-01",
				Type = WorkflowStepType.Scan,
				Name = "Сканирование",
				Order = 1,
				Description = "Сканируем коробки / паллеты / штуки",
				Actions =
				[ SendPackage(s1), NextStep(s1), CancelWorkflow(s1), ],
				Data = null
			},
			new()
			{
				Id = s2,
				WorkflowId = workflowId,
				StepCode = "Verify-02",
				Type = WorkflowStepType.Verify,
				Name = "Проверка",
				Order = 2,
				Description = "Сверка фактического кол-ва с документом",
				Actions = [ NextStep(s2), PrevStep(s2), CancelWorkflow(s2)],
				Data = null
			},
			new() 
			{ 
				Id = s3, 
				WorkflowId = workflowId, 
				StepCode = "Accept-03", 
				Type = WorkflowStepType.Accept,  
				Name = "Принятие на склад", 
				Order = 3, 
				Description = "Подтверждение и размещение", 
				Actions = [ PrevStep(s3), CompleteWorkflow(s3), CancelWorkflow(s3)], 
				Data = null
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

	private static List<WorkflowStep> SeedReturnSteps(Guid workflowId, Guid s1, Guid s2, Guid s3)
	{
		return new List<WorkflowStep>
		{
			new()
			{
				Id = s1,
				WorkflowId = workflowId,
				StepCode = "Scan-01",
				Type = WorkflowStepType.Scan,
				Name = "Скан возврата",
				Description = "",
				Order = 1,
				Actions = [ SendPackage(s1), NextStep(s1) ],
				Data = null
			},
			new()
			{
				Id = s2,
				WorkflowId = workflowId,
				StepCode = "Verify-02",
				Type = WorkflowStepType.Verify,
				Name = "Проверка качества",
				Description = "",
				Order = 2,
				Actions = [ NextStep(s2), PrevStep(s2) ],
				Data = null
			},
			new() 
			{ 
				Id = s3, 
				WorkflowId = workflowId, 
				StepCode = "Accept-03", 
				Type = WorkflowStepType.Accept, 
				Name = "Приём", 
				Description = "", 
				Order = 3, 
				Actions = [PrevStep(s3), CompleteWorkflow(s3) ], 
				Data = null
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
		Endpoint = "NextWorkflowStep",
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

