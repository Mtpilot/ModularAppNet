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
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Domain.Entities;
//using Modules.Workflows.Infrastructure.Database;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.Features.Features.SendScannedBarcodes;

internal interface ISendScannedBarcodesHandler : IHandler
{
	Task<Result<int>> HandleAsync(string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken);
}


internal sealed class SendScannedBarcodesHandler(
	WorkflowsDbContext context,
	ILogger<SendScannedBarcodesHandler> logger
	) : ISendScannedBarcodesHandler
{
	public async Task<Result<int>> HandleAsync(string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken)
	{
		//SESZH: будем ли мы сообщать клиенту о статусе [обработки отсканированных штрихкодов? Если да, то нужно будет добавить в ответ информацию о том, что штрихкоды были успешно обработаны или произошла ошибка.] до сих пор жутко, что нейронки так генерируют комментарии по контексту.
		logger.LogInformation("");

		var step = await context.WorkflowSteps
			//.Include(x => x.Data)
			.FirstOrDefaultAsync(x => x.StepCode == stepCode, cancellationToken);

		var workflow = await context.Workflows.Include(w => w.Steps).FirstOrDefaultAsync(w => w.Id == step.WorkflowId, cancellationToken);

		var nextStep = workflow?.Steps.FirstOrDefault(s => s.Order == step.Order + 1); //SESZH: пока мы уверены, что шаг подтверждения будет следующим - будет так

		if (step is null)
		{
			throw new NotSupportedException("Step not found"); //SESZH: пока не знаю, в каких случаях может быть неверный код и что с этим делать.
		}
		var data = step.Data;
		var found = 0;
		foreach (var payload in body)
		{
			var item = (step.Data as DefaultWorkflowDataCollection<Invoice>)?.Collection.First().Lines.FirstOrDefault(i => i.Barcode.Equals(payload.Barcode, StringComparison.OrdinalIgnoreCase));
			if (item != null)
			{
				found++;
			}
		}

		await context.SaveChangesAsync(cancellationToken);
		//SESZH: пусть пока это возвращается
		return found;
	}
}
