using System.Text.Json;

namespace Modules.Workflows.Features.Features.Shared.Requests;

/// <summary>
/// Request body for advancing workflow to the next step.
/// </summary>
public sealed record WorkflowNextStepBodyRequest
{
	/// <summary>
	/// Arbitrary JSON data structure that can include objects, arrays, and nested structures.
	/// Use JsonElement to access the data: request.Data.GetProperty("key"), request.Data.EnumerateArray(), etc.
	/// </summary>
	public required JsonElement Data { get; set; }

}

