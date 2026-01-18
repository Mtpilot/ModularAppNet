using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Features.Features.Shared.Requests;

public sealed record WorkflowNextStepBodyRequest
{
	public required Dictionary<string, object> Data { get; set; }
}

