using Microsoft.AspNetCore.Authorization;
using Modules.Common.Infrastructure.Policies;
using Modules.MaintenanceOperations.Domain.Policies;

namespace Modules.MaintenanceOperations.Infrastructure.Policies;

internal sealed class MaintenanceOperationsPolicyFactory : IPolicyFactory
{
	public string ModuleName => "MaintenanceOperations";

	public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
	{
		return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
		{
			[MaintenanceOperationsPolicyConstants.ReadPolicy] = policy => policy.RequireClaim(MaintenanceOperationsPolicyConstants.ReadPolicy),
			[MaintenanceOperationsPolicyConstants.CreatePolicy] = policy => policy.RequireClaim(MaintenanceOperationsPolicyConstants.CreatePolicy),
			[MaintenanceOperationsPolicyConstants.UpdatePolicy] = policy => policy.RequireClaim(MaintenanceOperationsPolicyConstants.UpdatePolicy),
			[MaintenanceOperationsPolicyConstants.DeletePolicy] = policy => policy.RequireClaim(MaintenanceOperationsPolicyConstants.DeletePolicy)
		};
	}
}
