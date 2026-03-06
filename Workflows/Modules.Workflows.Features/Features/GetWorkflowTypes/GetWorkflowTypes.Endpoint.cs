using Microsoft.AspNetCore.Builder;
using Modules.Common.API.Abstractions;
using Modules.Workflows.Features.Features.Shared.Routes;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Domain.Policies;
using Microsoft.AspNetCore.Http;
using Modules.Common.API.Extensions;

namespace Modules.Workflows.Features.Features.GetWorkflowTypes;

public class GetWorkflowTypesApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapGet(RouteConsts.GetWorkflowTypes, Handle)
			.WithName(EndpointConsts.GetWorkflowTypes)
			.WithTags("Workflow group")
			.WithSummary("Get all workflow types")
			.WithDescription("Сначала получаем все типы воркфлоу, а потом для конкретного типа выбираем активные воркфлоу (например тип \"Приемка из Машины\", конкретный воркфлоу это процесс с прикрепленной накладной. Таких процессов N на один тип воркфлоу)")
			.RequireAuthorization(WorkflowPolicyConsts.ReadPolicy)
			.Produces<List<WorkflowTypesResponse>>(StatusCodes.Status200OK);
	}

	/// <summary>
	/// Сначала получаем все типы воркфлоу, а потом для конкретного типа выбираем активные воркфлоу (например тип "Приемка из Машины", конкретный
	/// воркфлоу это процесс с прикрепленной накладной. Таких процессов N на один тип воркфлоу)
	/// </summary>
	private static async Task<IResult> Handle(
		IGetWorkflowTypes handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
