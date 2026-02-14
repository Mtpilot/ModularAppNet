using Microsoft.AspNetCore.Authorization;
using Modules.Common.Infrastructure.Policies;
//using Modules.Workflows.Domain.Policies;

//namespace Modules.Workflows.Infrastructure.Policies;

//internal sealed class WorkflowsPolicyFactory : IPolicyFactory
//{
//    public string ModuleName => "Workflows";
//
//    public Dictionary<string, Action<AuthorizationPolicyBuilder>> GetPolicies()
//    {
//        return new Dictionary<string, Action<AuthorizationPolicyBuilder>>
//        {
//            [WorkflowPolicyConstants.ReadPolicy] = policy => policy.RequireClaim(WorkflowPolicyConstants.ReadPolicy),
//            [WorkflowPolicyConstants.CreatePolicy] = policy => policy.RequireClaim(WorkflowPolicyConstants.CreatePolicy),
//            [WorkflowPolicyConstants.UpdatePolicy] = policy => policy.RequireClaim(WorkflowPolicyConstants.UpdatePolicy),
//            [WorkflowPolicyConstants.DeletePolicy] = policy => policy.RequireClaim(WorkflowPolicyConstants.DeletePolicy)
//        };
//    }
//}
