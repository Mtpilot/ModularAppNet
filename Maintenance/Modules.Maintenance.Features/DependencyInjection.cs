using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class MaintenanceModuleRegistration
{
	public static IServiceCollection AddMaintenanceModule(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddMaintenanceModuleApi()
			.AddMaintenanceInfrastructure(configuration);
	}

	private static IServiceCollection AddMaintenanceModuleApi(this IServiceCollection services)
	{
		services.RegisterApiEndpointsFromAssemblyContaining(typeof(MaintenanceModuleRegistration));

		services.RegisterHandlersFromAssemblyContaining(typeof(MaintenanceModuleRegistration));

		services.AddValidatorsFromAssembly(typeof(MaintenanceModuleRegistration).Assembly);

		return services;
	}
}

//SESZH: я пока не понимаю, что это
//public class StocksMiddlewareConfigurator : IModuleMiddlewareConfigurator
//{
//	public IApplicationBuilder Configure(IApplicationBuilder app)
//	{
//		return app.UseMiddleware<CheckRevocatedTokensMiddleware>();
//	}
//}


