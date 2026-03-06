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
    IInMemoryDbHelper mockTmpHelper) : IGetWorkflowHandler
{
	public async Task<Result<WorkflowResponse>> HandleAsync(string workflowCode, CancellationToken cancellationToken)
	{
		logger.LogInformation($"Getting workflow {workflowCode}");

        	var workflow = await context.Workflows
        		.Include(w=> w.Type)
        .Include(w => w.Steps)
        	.ThenInclude(s => s.Actions)
        .FirstOrDefaultAsync(w => w.Code == workflowCode, cancellationToken);
        if(workflow == null)
        {
            return WorkflowErrors.NotFound(workflowCode);
        }
        var currentStep = workflow.CurrentStep();
        var workflowData = new WorkflowDataCollection<IBaseWorkflowDataDto> { Name = workflow.Name, Description = workflow.Description, Collection = await mockTmpHelper.GetWorkflowDataFromInMemoryDb(workflow.Id, cancellationToken) };
        var stepData = new WorkflowDataCollection<IBaseStepDataDto> { Name = currentStep.Name, Description = currentStep.Description, Collection = await mockTmpHelper.GetStepDataFromInMemoryDb(currentStep.Id, currentStep.Type.ToString(), cancellationToken) };
        return workflow.ConvertWorkflowToResponse(linkService, workflowData, stepData);
	}
}