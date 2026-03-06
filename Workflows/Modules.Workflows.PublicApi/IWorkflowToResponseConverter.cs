using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.PublicApi.Contracts;
using Modules.Workflows.PublicApi.Responses;

namespace Modules.Workflows.PublicApi;

/// <summary>
/// Converts workflow and step data into <see cref="WorkflowResponse"/>.
/// Implementation is provided by the Workflows module; other modules (e.g. Barcoding) use it via DI.
/// </summary>
public interface IWorkflowToResponseConverter
{
	/// <summary>
	/// Loads workflow by code and builds response with invoice step data.
	/// </summary>
	Task<WorkflowResponse> ConvertAsync(
		string workflowCode,
		ILinkService linkService,
		IReadOnlyList<IBaseWorkflowDataDto> workflowData,
		IReadOnlyList<IBaseStepDataDto> stepData,
		CancellationToken cancellationToken = default);
}
