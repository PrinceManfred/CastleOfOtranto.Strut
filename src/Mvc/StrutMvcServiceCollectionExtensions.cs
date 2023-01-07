using System;
using CastleOfOtranto.Strut;
using CastleOfOtranto.Strut.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class StrutMvcServiceCollectionExtensions
{
	public static IServiceCollection AddStrutMvcApiDescriptionProvider(this IServiceCollection services)
	{
		services.TryAddEnumerable(
			ServiceDescriptor.Transient<IApiDescriptionProvider
				,RequiredParametersApiDescriptionProvider>());

		return services;
	}

	public static IServiceCollection AddStrutMvc(this IServiceCollection services)
	{
		services.TryAddSingleton<TypeInfoCache>();

		services.AddOptions<MvcOptions>().Configure<TypeInfoCache>((options, cache) =>
		{
			options.ModelMetadataDetailsProviders.Add(new StrutValidationMetadataProvider(cache));
			options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
		});

		return services;
	}
}

