using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Features.Features.Shared.Responses;


//TODO: должен возвращаться WorkflowType, а не Code конкретного Workflow
public sealed record WorkflowTypesResponse(string WorkflowTypeCode, string Name);



