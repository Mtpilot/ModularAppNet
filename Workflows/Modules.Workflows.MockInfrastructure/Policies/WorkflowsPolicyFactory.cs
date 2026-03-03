using Microsoft.AspNetCore.Authorization;
using Modules.Common.Infrastructure.Policies;
using Modules.Workflows.Domain.Policies;

    namespace Modules.Workflows.MockInfrastructure.Policies;

public sealed class WorkflowsPolicyFactory : IPolicyFactory
{
    public string ModuleName => "Workflows";
    public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
    {
        return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
        {
            [WorkflowPolicyConsts.ReadPolicy] = policy => policy.RequireClaim(WorkflowPolicyConsts.ReadPolicy),
            [WorkflowPolicyConsts.CreatePolicy] = policy => policy.RequireClaim(WorkflowPolicyConsts.CreatePolicy),
            [WorkflowPolicyConsts.UpdatePolicy] = policy => policy.RequireClaim(WorkflowPolicyConsts.UpdatePolicy),
            [WorkflowPolicyConsts.DeletePolicy] = policy => policy.RequireClaim(WorkflowPolicyConsts.DeletePolicy)
        };
    }
}