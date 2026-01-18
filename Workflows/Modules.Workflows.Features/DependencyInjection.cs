using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class WorkflowsModuleRegistration
{
    public static IServiceCollection AddWorkflowsModule(this IServiceCollection services, IConfiguration _)
    {
        return services
            .AddWorkflowsModuleApi();
    }
    
    private static IServiceCollection AddWorkflowsModuleApi(this IServiceCollection services)
    {
        services.RegisterApiEndpointsFromAssemblyContaining(typeof(WorkflowsModuleRegistration));
        
        services.RegisterHandlersFromAssemblyContaining(typeof(WorkflowsModuleRegistration));
        
        services.AddValidatorsFromAssembly(typeof(WorkflowsModuleRegistration).Assembly);

        return services;
    }
}
