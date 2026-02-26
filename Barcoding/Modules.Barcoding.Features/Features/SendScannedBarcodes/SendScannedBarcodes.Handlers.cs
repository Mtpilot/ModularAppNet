using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.PublicApi;
using Modules.Workflows.PublicApi.Requests;
using Modules.Workflows.PublicApi.Responses;
using Modules.Barcoding.MockInfrastructure.Database;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Errors;
using Modules.Barcoding.Domain.Errors;

namespace Modules.Barcoding.Features.Features.SendScannedBarcodes;

internal interface ISendScannedBarcodesHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken);
}


internal sealed class SendScannedBarcodesHandler(
	BarcodingDbContext context,
	ILogger<SendScannedBarcodesHandler> logger,
	ILinkService linkService,
	IGetWorkflowMetadata getWorkflowMetadata,
	IGetStepMetadata getStepMetadata,
	IMockTmpHelper mockTmpHelper,
	IWorkflowToResponseConverter workflowToResponseConverter
	) : ISendScannedBarcodesHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodePayload> body, CancellationToken cancellationToken)
	{
		logger.LogInformation("Sending scanned barcodes for workflow '{WorkflowCode}' and step '{StepCode}'", workflowCode, stepCode);


		var workflow = await getWorkflowMetadata.GetWorkflowMetadataByCodeAsync(workflowCode, cancellationToken); // await context.Workflows.Include(w=> w.Type).Include(w => w.Steps).ThenInclude(s=> s.Actions).FirstOrDefaultAsync(w => w.Code == workflowCode, cancellationToken);
		if (workflow is null)
		{
			return WorkflowErrors.NotFound(workflowCode);
		}


		var step = await getStepMetadata.GetStepMetadataByCodesAsync(workflow.Id, stepCode, cancellationToken); //await context.WorkflowSteps.Include(s=> s.Actions).FirstOrDefaultAsync(x => x.StepCode == stepCode && x.WorkflowId == workflow.Id, cancellationToken);
        if (step is null)
        {
            return WorkflowErrors.StepNotFound(stepCode);
        }


		if(workflow.CurrentStepId != step.Id)
		{
			return WorkflowErrors.WrongStep(workflow.Code, workflow.CurrentStepType, "SendScannedBarcodes");
		}

		var nextStep = workflow?.Steps.FirstOrDefault(s => s.Order == step.Order + 1); //SESZH: пока мы уверены, что шаг подтверждения будет следующим - будет так
		if (nextStep is null)
		{
			return WorkflowErrors.NextStepNotFound(workflowCode, stepCode);
		}



		var stepData = await mockTmpHelper.GetMockInvoicesFromInMemoryDb(step.Id, cancellationToken);
        if (stepData is null)
		{
			return DataErrors.StepDataNotFound(workflowCode, stepCode);
		}

		var checkoutEntity = await context.InvoiceCheckouts.FirstOrDefaultAsync(x => x.StepId == nextStep.Id, cancellationToken);
		if (checkoutEntity is null)
		{
			return DataErrors.NextStepDataNotFound(workflowCode, nextStep.StepCode);
		}

		var lines = stepData[0].lines;
		var payloadByBarcode = body
			.GroupBy(p => p.Barcode, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(g => g.Key, g => g.Sum(p => p.Quantity), StringComparer.OrdinalIgnoreCase);

		var totalItems = lines.Sum(l => l.quantity);
		var acceptedItems = 0;
		var extraItems = 0;
		var missingItems = 0;

		foreach (var line in lines)
		{
			var expectedQty = line.quantity;
			var scannedQty = payloadByBarcode.GetValueOrDefault(line.barcode, 0);

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

		var tmpWorkflowData = await mockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(workflow.Id, cancellationToken);
		var tmpNextStepData = await mockTmpHelper.GetMockInvoiceCheckoutsFromInMemoryDb(workflow.NextStepId, cancellationToken);
		var response = await workflowToResponseConverter.ConvertAsync(workflowCode, linkService, tmpWorkflowData, tmpNextStepData, cancellationToken);
		return response;
	}
}
