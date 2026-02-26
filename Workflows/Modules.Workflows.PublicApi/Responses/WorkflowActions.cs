using Modules.Common.API.Abstractions.Links;

namespace Modules.Workflows.PublicApi.Responses;

/// <summary>
/// Доступные действия на текущем шаге.
/// </summary>
public sealed record WorkflowActions
{
	/// <summary>
	/// Действия для текущего шага (например, SendScannedBarcodes).
	/// </summary>
	public required IList<Link> StepActions { get; init; }

	/// <summary>
	/// Ссылка на переход к следующему шагу (если доступно).
	/// </summary>
	public Link? NextStep { get; set; }
}
