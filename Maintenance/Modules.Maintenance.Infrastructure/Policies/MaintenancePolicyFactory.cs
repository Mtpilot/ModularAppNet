using Microsoft.AspNetCore.Authorization;
using Modules.Common.Infrastructure.Policies;
using Modules.Maintenance.Domain.Policies;

namespace Modules.Maintenance.Infrastructure.Policies;

internal sealed class MaintenancePolicyFactory : IPolicyFactory
{
	public string ModuleName => "Maintenance";

	public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies() //SESZH: пока выглядит, как переусложнение, нужен пример, где оно такое перессылочное нужно
	{
		return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
		{
			[MaintenancePolicyConstants.ReadPolicy] = policy => policy.RequireClaim(MaintenancePolicyConstants.ReadPolicy),
			[MaintenancePolicyConstants.CreatePolicy] = policy => policy.RequireClaim(MaintenancePolicyConstants.CreatePolicy),
			[MaintenancePolicyConstants.UpdatePolicy] = policy => policy.RequireClaim(MaintenancePolicyConstants.UpdatePolicy),
			[MaintenancePolicyConstants.DeletePolicy] = policy => policy.RequireClaim(MaintenancePolicyConstants.DeletePolicy)
		};
	}
}
