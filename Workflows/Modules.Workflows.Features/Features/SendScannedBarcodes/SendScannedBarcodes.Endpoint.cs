using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.SendScannedBarcodes;

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
			.Produces<int>(StatusCodes.Status200OK);
	}

	private static async Task<IResult> Handle(
		[FromRoute] string stepCode,
		[FromBody] List<ScannedBarcodePayload> scannedBarcodes,
		ISendScannedBarcodesHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(stepCode, scannedBarcodes, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
