using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Workflows.Domain.Policies;

public static class WorkflowPolicyConsts
{
	public const string ReadPolicy = "workflows:read";
	public const string CreatePolicy = "workflows:create";
	public const string UpdatePolicy = "workflows:update";
	public const string DeletePolicy = "workflows:delete";
}
