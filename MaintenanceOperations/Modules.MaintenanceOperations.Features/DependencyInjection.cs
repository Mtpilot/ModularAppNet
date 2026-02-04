using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Common.API.Abstractions;
using Modules.Common.Application.Extensions;
using Modules.MaintenanceOperations.Features.InternalApi;
using Modules.MaintenanceOperations.PublicApi;

namespace Microsoft.Extensions.DependencyInjection;

public static class MaintenanceOperationsModuleRegistration
{
	public static IServiceCollection AddMaintenanceOperationsModule(this IServiceCollection services, IConfiguration configuration)
	{
		return services
			.AddMaintenanceOperationsModuleApi()
			.AddMaintenanceOperationsInfrastructure(configuration);
	}

	private static IServiceCollection AddMaintenanceOperationsModuleApi(this IServiceCollection services)
	{
		services.RegisterApiEndpointsFromAssemblyContaining(typeof(MaintenanceOperationsModuleRegistration));

		services.RegisterHandlersFromAssemblyContaining(typeof(MaintenanceOperationsModuleRegistration));

		services.AddValidatorsFromAssembly(typeof(MaintenanceOperationsModuleRegistration).Assembly);

		services.AddScoped<IMaintenanceOperationsModuleApi, OperationModuleApi>();

		return services;
	}
}


