using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Features.Features.Shared.Requests;

public sealed record ScannedBarcodePayload
{
	public required string Barcode { get; set; }
	public required int Quantity { get; set; }
}

