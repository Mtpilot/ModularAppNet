using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.Domain.Errors;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.PublicApi.Contracts;

namespace Modules.Workflows.Features.Features.GetWorkflow;

internal interface IGetWorkflowHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken);
}


internal sealed class GetWorkflowHandler(
	ILogger<GetWorkflowHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService,
    IMockTmpHelper mockTmpHelper) : IGetWorkflowHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting active workflows");

        	var workflow = await context.Workflows
        		.Include(w=> w.Type)
        .Include(w => w.Steps)
        	.ThenInclude(s => s.Actions)
        .FirstOrDefaultAsync(w => w.Code == workflowCode, cancellationToken);
        if(workflow == null)
        {
            return WorkflowErrors.NotFound(workflowCode);
        }
        var tmpWorkflowData = new DefaultWorkflowDataCollection<InvoiceHeaderDto> { Name = "InvoiceHeaders", Description = "Collection of invoice headers", Collection = await mockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(workflow.Id, cancellationToken) }; 
        switch(workflow.CurrentStep().Type) //SESZH: надо срочно доделывать сигнатуры и начинать очистку от этого всего, потом завязну, оно все нарастает
        {
            case WorkflowStepType.Scan:
            case WorkflowStepType.Accept:
                {
                    var tmpStepData = new DefaultWorkflowDataCollection<InvoiceDto> { Name = "Invoices", Description = "Collection of invoices", Collection = await mockTmpHelper.GetMockInvoicesFromInMemoryDb(workflow.CurrentStep().Id, cancellationToken) };
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
            case WorkflowStepType.Verify:
                {
                    var tmpStepData = new DefaultWorkflowDataCollection<InvoiceCheckoutDto> { Name = "InvoiceCheckouts", Description = "Collection of invoice checkouts", Collection = await mockTmpHelper.GetMockInvoiceCheckoutsFromInMemoryDb(workflow.CurrentStep().Id, cancellationToken) };
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
                default:
                throw new NotSupportedException("Current step type not supported");
        }
	}
}