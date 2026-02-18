using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
//using Modules.Workflows.Infrastructure.Database;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.Features.Features.SendScannedBarcodes;

internal interface ISendScannedBarcodesHandler : IHandler
{
	Task<Result<int>> HandleAsync(string workflowCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken);
}


internal sealed class SendScannedBarcodesHandler(
	WorkflowsDbContext context,
	ILogger<SendScannedBarcodesHandler> logger
	) : ISendScannedBarcodesHandler
{
	public async Task<Result<int>> HandleAsync(string workflowCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken)
	{
		//SESZH: будем ли мы сообщать клиенту о статусе [обработки отсканированных штрихкодов? Если да, то нужно будет добавить в ответ информацию о том, что штрихкоды были успешно обработаны или произошла ошибка.] до сих пор жутко, что нейронки так генерируют комментарии по контексту.
		logger.LogInformation("");

		var workflow = await context.Workflows
			.Include(x => x.Steps)
			.FirstOrDefaultAsync(x => x.Code == workflowCode, cancellationToken);

		if (workflow is null)
		{
			throw new NotSupportedException("Workflow not found"); //SESZH: пока не знаю, в каких случаях может быть неверный код и что с этим делать.
		}
		var scanStep = workflow.Steps.FirstOrDefault(s => string.Equals(s.Type, "Scan", StringComparison.OrdinalIgnoreCase));
		if (scanStep is null)
		{
			return 0;
		}
		var data = scanStep.Data;
		var changed = 0;
		foreach (var payload in body)
		{
			var itemDict = data.FirstOrDefault(d =>
				d.TryGetValue("ItemCode", out var code) && payload.Barcode.Equals(code?.ToString(), StringComparison.OrdinalIgnoreCase));
			if (itemDict is null)
			{
				continue;
			}
			itemDict["Quantity"] = payload.Quantity;
			changed++;
		}
		// Сохраняем изменения обратно в шаг, чтобы DataJson обновился перед SaveChanges
		if (changed > 0)
			scanStep.Data = data;

		await context.SaveChangesAsync(cancellationToken);
		//SESZH: пусть пока это возвращается
		return changed;

		//		var links = await context.WorkflowDataLinks
		//	.Where(l => workflow.DataGuids.Contains(l.DataId))
		//	.ToListAsync(cancellationToken);
		//
		//		var groups = links.GroupBy(l => l.DataType);
		//
		//		var itemIds = groups
		//	.Where(g => g.Key == "Item")
		//	.SelectMany(g => g.Select(x => x.DataId))
		//	.ToList();
		//
		//		var changedItems = await context.WorkflowDataItems
		//	.Where(x => itemIds.Contains(x.Id))
		//	.ExecuteUpdateAsync(setters => setters
		//		.SetProperty(e => e.Quantity, e => e.Quantity + 1),
		//		cancellationToken);
		//
		//		return changedItems; 

	}


}
