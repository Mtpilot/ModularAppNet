using FluentValidation;
using Microsoft.Extensions.Configuration;
using Modules.Common.Application.Extensions;
using Modules.Workflows.Features.Features.Shared;
//using Modules.Workflows.Infrastructure.Database;
using Modules.Workflows.PublicApi;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Workflows.Features.QueryHandlers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class WorkflowsModuleRegistration
{
    public static IServiceCollection AddWorkflowsModule(this IServiceCollection services, IConfiguration _)
    {
        return services
            .AddWorkflowsModuleApi()
			//.AddWorkflowsInfrastructure(_)
			.AddWorkflowsMockInfrastructure()
			;
    }
    
    private static IServiceCollection AddWorkflowsModuleApi(this IServiceCollection services)
    {
        services.RegisterApiEndpointsFromAssemblyContaining(typeof(WorkflowsModuleRegistration));
        
        services.RegisterHandlersFromAssemblyContaining(typeof(WorkflowsModuleRegistration));
        
        services.AddValidatorsFromAssembly(typeof(WorkflowsModuleRegistration).Assembly);

        services.AddScoped<IGetStepMetadata, GetStepMetadata>();
        services.AddScoped<IGetWorkflowMetadata, GetWorkflowMetadata>();
        services.AddScoped<IWorkflowToResponseConverter, WorkflowToResponseConverter>();

        return services;
    }
}
