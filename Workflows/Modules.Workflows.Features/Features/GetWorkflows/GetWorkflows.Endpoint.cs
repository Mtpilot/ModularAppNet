using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.GetWorkflows;

public class GetWorkflowsEndpoint : IApiEndpoint
{
    public void MapEndpoint(WebApplication app)
    {
        app.MapGet(RouteConsts.GetWorkflows, Handle)
            .WithName("GetWorkflows")
            .WithTags("Workflow group")
            .WithSummary("Get workflows by type")
            .WithDescription("Получить список ВСЕХ воркфлоу одного типа. ")
            .Produces<List<WorkflowShortInfoResponse>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(
        [FromRoute] string workflowTypeCode,
        IGetWorkflowsHandler handler,
        CancellationToken cancellationToken
        )
    {
        var response = await handler.HandleAsync(workflowTypeCode,  cancellationToken);
        if (response.IsError)
        {
            return response.Errors.ToProblem();
        }

        return Results.Ok(response.Value);
    }
}
