using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Modules.Common.API.Abstractions.Links;
using Modules.Common.Domain.Handlers;
using Modules.Common.Domain.Results;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.MockInfrastructure.Database;

namespace Modules.Workflows.Features.Features.GetWorkflowTypes;

internal interface IGetWorkflowTypes : IHandler
{
	Task<Result<List<WorkflowTypesResponse>>> HandleAsync( CancellationToken cancellationToken);
}


internal sealed class GetWorkflowTypesHandler(
	WorkflowsDbContext context,
	ILogger<GetWorkflowTypesHandler> logger) : IGetWorkflowTypes
{
	public async Task<Result<List<WorkflowTypesResponse>>> HandleAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Getting workflow types");

		var response = await context.WorkflowTypes.Select(x => new WorkflowTypesResponse(x.Code, x.Name)).ToListAsync(cancellationToken);

		//var response = new List<WorkflowTypesResponse>() {
		//	new WorkflowTypesResponse("ReceiveGoods", "Receive goods Workflow"),
		//	new WorkflowTypesResponse("ItemPlacement", "Item placement Workflow")
		//};


		return response;
	}
}

	
