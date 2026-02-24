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
using Modules.Workflows.Domain.Entities.Application;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Infrastructure.Helpers;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.Features.Features.Shared.Helpers;
using Modules.Workflows.Domain.Errors;
using HttpMethod = Modules.Common.API.Abstractions.Links.HttpMethod;

namespace Modules.Workflows.Features.Features.GetWorkflow;

internal interface IGetWorkflowHandler : IHandler
{
	Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken);
}


internal sealed class GetWorkflowHandler(
	ILogger<GetWorkflowHandler> logger,
	WorkflowsDbContext context,
	ILinkService linkService) : IGetWorkflowHandler
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
        var tmpWorkflowData = await MockTmpHelper.GetMockInvoiceHeadersFromInMemoryDb(context, workflow.Id, cancellationToken); 
        switch(workflow.CurrentStep().Type) //SESZH: надо срочно доделывать сигнатуры и начинать очистку от этого всего, потом завязну, оно все нарастает
        { 
            case WorkflowStepType.Scan:
                {
                    var tmpStepData = await MockTmpHelper.GetMockInvoicesFromInMemoryDb(context, workflow.CurrentStep().Id, cancellationToken);
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
            case WorkflowStepType.Verify:
                {
                    var tmpStepData = await MockTmpHelper.GetMockInvoiceCheckoutsFromInMemoryDb(context, workflow.CurrentStep().Id, cancellationToken);
                    var wf = workflow.ConvertWorkflowToResponse(linkService, tmpWorkflowData, tmpStepData);
                    return wf;
                }
                default:
                throw new NotSupportedException("Current step type not supported");
        }       	
	} 
}