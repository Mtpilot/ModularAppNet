using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.PublicApi.Responses;
using Modules.Barcoding.Features.Features.Shared.Routes;
using Modules.Barcoding.Domain.Policies;
using Modules.Barcoding.Features.Requests;

namespace Modules.Barcoding.Features.Features.SendScannedBarcodes;

public class SendScannedBarcodesApiEndpoint : IApiEndpoint
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.SendScannedBarcodes, Handle)
			.WithName(EndpointConsts.SendScannedBarcodes)
			.WithTags("Workflow group")
			.WithSummary("Send scanned barcodes with quantities")
			.WithDescription("Отправить отсканированные штрих-коды с количествами.")
			.RequireAuthorization(BarcodingPolicyConsts.UpdatePolicy)
			.Produces<WorkflowResponse>(StatusCodes.Status200OK)
			;
	}

	private static async Task<IResult> Handle(
		[FromRoute] string workflowCode,
		[FromRoute] string stepCode,
		[FromBody] List<ScannedBarcodesPayload> scannedBarcodes,
		ISendScannedBarcodesHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(workflowCode, stepCode, scannedBarcodes, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
