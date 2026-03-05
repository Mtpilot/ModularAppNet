using Microsoft.EntityFrameworkCore;
using Modules.Workflows.PublicApi.Contracts;
using Modules.Common.API.Abstractions.Links;
using Modules.Workflows.Domain.Entities;
using Modules.Workflows.MockInfrastructure.Database;
using Modules.Workflows.PublicApi;
using Modules.Workflows.PublicApi.Responses;
using Modules.Workflows.Features.Features.Shared.Helpers;

namespace Modules.Workflows.Features.Features.Shared;

internal sealed class WorkflowToResponseConverter(WorkflowsDbContext context) : IWorkflowToResponseConverter
{
	public async Task<WorkflowResponse> ConvertAsync(
		string workflowCode,
		ILinkService linkService,
		IReadOnlyList<IBaseWorkflowDataDto> workflowData,
		IReadOnlyList<IBaseStepDataDto> stepData,
		CancellationToken cancellationToken = default)
	{
		var workflow = await LoadWorkflowAsync(workflowCode, cancellationToken);
		var workflowDataCol = new WorkflowDataCollection<IBaseWorkflowDataDto>
		{
			Name = "InvoiceHeaders",
			Description = "Collection of invoice headers",
			Collection = workflowData.ToList()
		};
		var stepDataCol = new WorkflowDataCollection<IBaseStepDataDto>
		{
			Name = "Invoices",
			Description = "Collection of invoices",
			Collection = stepData.ToList()
		};
		return workflow.ConvertWorkflowToResponse(linkService, workflowDataCol, stepDataCol);
	}

	//public async Task<WorkflowResponse> ConvertAsync(
	//	string workflowCode,
	//	ILinkService linkService,
	//	IReadOnlyList<IBaseWorkflowDataDto> workflowData,
	//	IReadOnlyList<IBaseStepDataDto> stepData,
	//	CancellationToken cancellationToken = default)
	//{
	//	var workflow = await LoadWorkflowAsync(workflowCode, cancellationToken);
	//	var workflowDataCol = new WorkflowDataCollection<IBaseWorkflowDataDto>
	//	{
	//		Name = "InvoiceHeaders",
	//		Description = "Collection of invoice headers",
	//		Collection = workflowData.ToList()
	//	};
	//	var stepDataCol = new WorkflowDataCollection<IBaseStepDataDto>
	//	{
	//		Name = "InvoiceCheckouts",
	//		Description = "Collection of invoice checkouts",
	//		Collection = stepData.ToList()
	//	};
	//	return workflow.ConvertWorkflowToResponse(linkService, workflowDataCol, stepDataCol);
	//}

	private async Task<Workflow> LoadWorkflowAsync(string workflowCode, CancellationToken cancellationToken)
	{
		var workflow = await context.Workflows
			.Include(w => w.Type)
			.Include(w => w.Steps)
			.ThenInclude(s => s.Actions)
			.FirstOrDefaultAsync(w => w.Code == workflowCode, cancellationToken);
		if (workflow is null)
			throw new InvalidOperationException($"Workflow with code '{workflowCode}' not found.");
		return workflow;
	}
}
