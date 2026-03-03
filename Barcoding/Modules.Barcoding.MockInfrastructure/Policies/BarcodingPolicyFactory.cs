using Microsoft.AspNetCore.Authorization;
using Modules.Common.Infrastructure.Policies;
using Modules.Barcoding.Domain.Policies;

namespace Modules.Barcoding.MockInfrastructure.Policies;

internal sealed class BarcodingPolicyFactory : IPolicyFactory
{
    public string ModuleName => "Barcoding";
    public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
    {
        return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
        {
            [BarcodingPolicyConsts.ReadPolicy] = policy => policy.RequireClaim(BarcodingPolicyConsts.ReadPolicy),
            [BarcodingPolicyConsts.CreatePolicy] = policy => policy.RequireClaim(BarcodingPolicyConsts.CreatePolicy),
            [BarcodingPolicyConsts.UpdatePolicy] = policy => policy.RequireClaim(BarcodingPolicyConsts.UpdatePolicy),
            [BarcodingPolicyConsts.DeletePolicy] = policy => policy.RequireClaim(BarcodingPolicyConsts.DeletePolicy)
        };
    }
}
