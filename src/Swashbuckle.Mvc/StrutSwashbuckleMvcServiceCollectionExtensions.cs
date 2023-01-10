using CastleOfOtranto.Strut;
using CastleOfOtranto.Strut.Mvc;
using CastleOfOtranto.Strut.Swashbuckle;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class StrutSwashbuckleMvcServiceCollectionExtensions
{
    public static IServiceCollection AddStrutSwashbuckleMvc(this IServiceCollection services)
    {
        services.TryAddSingleton<TypeInfoCache>();

        services.AddOptions<MvcOptions>().Configure<TypeInfoCache>((options, cache) =>
        {
            options.ModelMetadataDetailsProviders.Add(new StrutValidationMetadataProvider(cache));
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });

        services.AddOptions<SwaggerGenOptions>().Configure<TypeInfoCache>((o, cache) =>
        {
            o.SupportNonNullableReferenceTypes();
            o.ParameterFilter<RequiredParameterFilter>();
            o.SchemaFilter<RequiredPropertySchemaFilter>(cache);
            o.RequestBodyFilter<RequiredRequestBodyFilter>();
            o.OperationFilter<ReadOnlyOperationFilter>();
            o.UseAllOfToExtendReferenceSchemas();
        });

        return services;
    }
}