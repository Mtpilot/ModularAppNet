using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.PublicApi;
using Modules.Workflows.PublicApi.Responses;
using Modules.Barcoding.MockInfrastructure.Database;
using Modules.Barcoding.Features.Contracts;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Errors;
using Modules.Barcoding.Domain.Errors;
using Modules.Barcoding.Domain.Enums;
using Modules.Barcoding.Features.Requests;
using Modules.Barcoding.Features.Features.Shared.Routes;

namespace Modules.Barcoding.Features.Features.SendScannedBarcodes;

internal interface ISendScannedBarcodesHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodesPayload> body, CancellationToken cancellationToken);
}


internal sealed class SendScannedBarcodesHandler(
	BarcodingDbContext context,
	ILogger<SendScannedBarcodesHandler> logger,
	ILinkService linkService,
	IGetWorkflowMetadata getWorkflowMetadata,
	IGetStepMetadata getStepMetadata,
	IInMemoryDbHelper mockTmpHelper,
	IWorkflowToResponseConverter workflowToResponseConverter
	) : ISendScannedBarcodesHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, string stepCode, List<ScannedBarcodesPayload> body, CancellationToken cancellationToken)
	{
		logger.LogInformation("Sending scanned barcodes for workflow '{WorkflowCode}' and step '{StepCode}'", workflowCode, stepCode);


		var workflow = await getWorkflowMetadata.GetWorkflowMetadataByCodeAsync(workflowCode, cancellationToken);
		if (workflow is null)
		{
			return WorkflowErrors.NotFound(workflowCode);
		}


		var step = await getStepMetadata.GetStepMetadataByCodesAsync(workflow.Id, stepCode, cancellationToken);
        if (step is null)
        {
            return WorkflowErrors.StepNotFound(stepCode);
        }
		//SESZH: надо подумать над проверками, а тут работать через типы шагов
		if(workflow.CurrentStepId != step.Id)
		{
			return WorkflowErrors.WrongStep(workflow.Code, workflow.CurrentStepType, EndpointConsts.SendScannedBarcodes);
		}
		if(!workflow.CurrentStepType.Equals(nameof(WorkflowStepType.Scan), StringComparison.OrdinalIgnoreCase))
		{
			return WorkflowErrors.WrongStep(workflow.Code, workflow.CurrentStepType, EndpointConsts.SendScannedBarcodes);
		}

		var nextStep = workflow.Steps.FirstOrDefault(s => s.Type.Equals(nameof(WorkflowStepType.Verify), StringComparison.OrdinalIgnoreCase)); //SESZH: теперь это работает только для баркодинга, в котором мы точно знаем, какой у шагов порядок и какие типы
		if (nextStep is null)
		{
			return WorkflowErrors.NextStepNotFound(workflowCode, stepCode);
		}

		var stepData = await mockTmpHelper.GetStepDataFromInMemoryDb(step.Id, step.Type, cancellationToken);
		if (stepData is null || stepData.Count == 0)
		{
			return DataErrors.StepDataNotFound(workflowCode, stepCode);
		}

		if (stepData[0] is not InvoiceDto scanStepData)
		{
			return DataErrors.StepDataNotFound(workflowCode, stepCode);
		}

		var checkoutEntity = await context.InvoiceCheckouts.FirstOrDefaultAsync(x => x.StepId == nextStep.Id, cancellationToken);
		if (checkoutEntity is null)
		{
			return DataErrors.NextStepDataNotFound(workflowCode, nextStep.StepCode);
		}

		var lines = scanStepData.lines;
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

		var tmpWorkflowData = await mockTmpHelper.GetWorkflowDataFromInMemoryDb(workflow.Id, cancellationToken);
		var tmpNextStepData = await mockTmpHelper.GetStepDataFromInMemoryDb(nextStep.Id, nextStep.Type, cancellationToken);
		var response = await workflowToResponseConverter.ConvertAsync(workflowCode, linkService, tmpWorkflowData, tmpNextStepData, cancellationToken);
		return response;
	}
}
