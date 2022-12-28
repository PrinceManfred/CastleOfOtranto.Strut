using System;
using CastleOfOtranto.Strut.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StrutMvcServiceCollectionExtensions
{
	public static IServiceCollection AddStrutMvcRequiredParameters(this IServiceCollection services)
	{
		services.TryAddEnumerable(
			ServiceDescriptor.Transient<IApiDescriptionProvider
				,RequiredParametersApiDescriptionProvider>());

		return services;
	}
}

