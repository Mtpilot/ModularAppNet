using Modules.Barcoding.MockInfrastructure.Database;
using Modules.Barcoding.Features.Contracts;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Modules.Barcoding.Features.Helpers;

//Временный класс для моков.
//SESZH: Есть мысль уже переименовать его из временного и для моков в постоянный. Все-таки он в своем модуле, и вполне решает задачу: "достать данные из бд". Оставлю темповым моковым, пока не определимся лучше с хранением данных
public class MockTmpHelper(BarcodingDbContext context) : IMockTmpHelper
{
	public async Task<List<IBaseWorkflowDataDto>> GetMockWorkflowDataFromInMemoryDb(Guid workflowId, CancellationToken cancellationToken)
	{
		var headers = await context.Invoices.Where(x => x.WorkflowId == workflowId).Select(x => new InvoiceHeaderDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date)).ToListAsync(cancellationToken);
		return headers.Cast<IBaseWorkflowDataDto>().ToList();
	}
	public async Task<List<IBaseStepDataDto>> GetMockStepDataFromInMemoryDb(Guid stepId, string stepType, CancellationToken cancellationToken)
	{
		return stepType switch
		{
			"Scan" => (await context.Invoices.Where(x => x.StepId == stepId).Select(x => new InvoiceDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date, x.Lines.Count, x.Lines.Select(y => new InvoiceLineDto(y.ProductName, y.ConstructorName, y.Barcode, y.Barcode, y.Quantity, y.Units)).ToList())).ToListAsync(cancellationToken)).Cast<IBaseStepDataDto>().ToList(),
			"Verify" => (await context.InvoiceCheckouts.Where(x => x.StepId == stepId).Select(x => new InvoiceCheckoutDto(x.InvoiceNumber, x.Counterparty, x.ContractNumber, x.Date, x.TotalItems, x.AcceptedItems, x.MissingItems, x.ExtraItems)).ToListAsync(cancellationToken)).Cast<IBaseStepDataDto>().ToList(),
			"Accept" => new List<IBaseStepDataDto>(),
			_ => throw new ArgumentException("Invalid step type"),
		};
	}
}
