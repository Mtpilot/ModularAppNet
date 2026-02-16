using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Modules.Workflows.Domain.Entities;

[JsonDerivedType(typeof(DataItemInventory), "inventory")]
public class DataItemInventory : IDataItem
{
	public required Guid Id { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required string ItemCode { get; set; }
	public required int Quantity { get; set; }
	public required string Units {  get; set; }
	public required decimal Price { get; set; }
	public string [] Tags { get; set; } = [];
}
