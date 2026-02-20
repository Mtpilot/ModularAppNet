using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Infrastructure.Helpers;
using Modules.Workflows.MockInfrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Workflows.Domain.Errors;

namespace Modules.Workflows.Features.Features.SendScannedBarcodes;

internal interface ISendScannedBarcodesHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken);
}


internal sealed class SendScannedBarcodesHandler(
	WorkflowsDbContext context,
	ILogger<SendScannedBarcodesHandler> logger,
	ILinkService linkService
    ) : ISendScannedBarcodesHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken)
	{
		//SESZH: будем ли мы сообщать клиенту о статусе [обработки отсканированных штрихкодов? Если да, то нужно будет добавить в ответ информацию о том, что штрихкоды были успешно обработаны или произошла ошибка.] до сих пор жутко, что нейронки так генерируют комментарии по контексту.
		logger.LogInformation("");


		var workflow = await context.Workflows.Include(w=> w.Type).Include(w => w.Steps).ThenInclude(s=> s.Actions).FirstOrDefaultAsync(w => w.Code == workflowCode, cancellationToken);
		if (workflow is null)
		{
			throw new NotSupportedException("Workflow not found"); //SESZH: пока не знаю, в каких случаях может быть неверный код и что с этим делать.
		}


		var step = await context.WorkflowSteps
			.Include(s=> s.Actions)
			//.Include(x => x.Data)
			.FirstOrDefaultAsync(x => x.StepCode == stepCode && x.WorkflowId == workflow.Id, cancellationToken);
        if (step is null)
        {
            throw new NotSupportedException("Step not found"); //SESZH: пока не знаю, в каких случаях может быть неверный код и что с этим делать.
        }


		if(workflow.CurrentStep().Id != step.Id)
		{
			return WorkflowErrors.WrongStep(workflow.Code, workflow.CurrentStep().Type.ToString(), "SendScannedBarcodes");
		}

		var nextStep = workflow?.Steps.FirstOrDefault(s => s.Order == step.Order + 1); //SESZH: пока мы уверены, что шаг подтверждения будет следующим - будет так
		if (nextStep is null)
		{
			throw new NotSupportedException("Next step not found"); //SESZH: пока не знаю, в каких случаях может быть неверный код и что с этим делать.
		}



		var stepData = await MockTmpHelper.GetMockInvoicesFromInMemoryDb(context, step.Id, cancellationToken);
        if (stepData is null)
		{
			throw new NotSupportedException("Step data not found");
		}

		var checkoutEntity = await context.InvoiceCheckouts.FirstOrDefaultAsync(x => x.StepId == nextStep.Id, cancellationToken);
		if (checkoutEntity is null)
		{
			throw new NotSupportedException("Next step data (checkout) not found");
		}

		var lines = stepData.Collection.First().Lines;
		var payloadByBarcode = body
			.GroupBy(p => p.Barcode, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(g => g.Key, g => g.Sum(p => p.Quantity), StringComparer.OrdinalIgnoreCase);

		var totalItems = (int)lines.Sum(l => l.Quantity);
		var acceptedItems = 0;
		var extraItems = 0;
		var missingItems = 0;

		foreach (var line in lines)
		{
			var expectedQty = (int)line.Quantity;
			var scannedQty = payloadByBarcode.GetValueOrDefault(line.Barcode, 0);

			if (scannedQty == expectedQty)
			{
				acceptedItems += scannedQty;
			}
			else if (scannedQty > expectedQty)
			{
				acceptedItems += expectedQty;
				extraItems += scannedQty - expectedQty;
			}
			else
			{
				acceptedItems += scannedQty;
				missingItems += expectedQty - scannedQty;
			}
		}


		checkoutEntity.TotalItems = totalItems;
		checkoutEntity.AcceptedItems = acceptedItems;
		checkoutEntity.MissingItems = missingItems;
		checkoutEntity.ExtraItems = extraItems;

		await context.SaveChangesAsync(cancellationToken);

		var tmpWorkflowData = await MockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(context, workflow.Id, cancellationToken);
		var tmpNextStepData = await MockTmpHelper.GetMockInvoiceCheckoutsFromInMemoryDb(context, workflow.GetNextStep(workflow.CurrentStepNumber).Id, cancellationToken);
		var response = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpNextStepData);
		return response;
	}
}
