using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.PublicApi.Requests;
using Modules.Workflows.PublicApi.Responses;
using Modules.Barcoding.Features.Features.Shared.Routes;
using Modules.Barcoding.Domain.Policies;

namespace Modules.Barcoding.Features.Features.SendScannedBarcodes;

#pragma warning disable MA0049 // Type name should not match containing namespace
public class SendScannedBarcodes : IApiEndpoint
#pragma warning restore MA0049 // Type name should not match containing namespace
{
	public void MapEndpoint(WebApplication app)
	{
		app.MapPatch(RouteConsts.SendScannedBarcodes, Handle)
			.WithName("SendScannedBarcodes")
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
		[FromBody] List<ScannedBarcodePayload> scannedBarcodes,
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
