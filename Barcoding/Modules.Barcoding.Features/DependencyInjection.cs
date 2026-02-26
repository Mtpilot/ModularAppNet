using Microsoft.Extensions.Configuration;
using Modules.Barcoding.Features.Helpers;
using Modules.Workflows.PublicApi.InfrastructureQueryInterfaces;
using Modules.Common.Application.Extensions;


namespace Microsoft.Extensions.DependencyInjection;

public static class BarcodingModuleRegistration
{
	public static IServiceCollection AddBarcodingModule(this IServiceCollection services, IConfiguration _)
	{
		return services
			.AddBarcodingModuleApi()
			//.AddBarcodingInfrastructure()
            .AddBarcodingMockInfrastructure();
	}
    private static IServiceCollection AddBarcodingModuleApi(this IServiceCollection services)
    {
        services.RegisterApiEndpointsFromAssemblyContaining(typeof(BarcodingModuleRegistration));
        services.RegisterHandlersFromAssemblyContaining(typeof(BarcodingModuleRegistration));
        //services.AddValidatorsFromAssembly(typeof(BarcodingModuleRegistration).Assembly);
        services.AddScoped<IMockTmpHelper, MockTmpHelper>();
        return services;
    }
}
