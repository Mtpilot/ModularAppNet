using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Common.API.Abstractions;
using Modules.Common.API.Extensions;
using Modules.Workflows.Features.Features.GetActiveWorkflows;
using Modules.Workflows.Features.Features.Shared.Requests;
using Modules.Workflows.Features.Features.Shared.Responses;
using Modules.Workflows.Features.Features.Shared.Routes;

namespace Modules.Workflows.Features.Features.SendScannedBarcodes;

public class SendScannedBarcodes : IApiEndpoint
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
		[FromRoute] string code,
		[FromBody] List<ScannedBarcodePayload> scannedBarcodes,
		ISendScannedBarcodesHandler handler,
		CancellationToken cancellationToken)
	{
		var response = await handler.HandleAsync(code, scannedBarcodes, cancellationToken);
		if (response.IsError)
		{
			return response.Errors.ToProblem();
		}

		return Results.Ok(response.Value);
	}
}
