using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Entities;

public class WorkflowDataItem : IDataItem
{
	public required Guid Id { get; set; }              
	public required Guid WorkflowId { get; set; }      
	public required string Name { get; set; }
	public required string Description { get; set; }
	//SESZH: задергался глаз, когда подумал, что придется вытаскивать из JsonData КАЖДЫЙ баркод и количество!!
	public required string Code { get; set; }
	public required int Quantity { get; set; }
	public required string Discriminator { get; set; }  
	public required string JsonData { get; set; }      

	public Workflow<IDataItem> Workflow { get; set; } = null!;
}
