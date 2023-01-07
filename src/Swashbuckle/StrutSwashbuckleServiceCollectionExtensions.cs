using System;
using CastleOfOtranto.Strut;
using CastleOfOtranto.Strut.Swashbuckle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Microsoft.Extensions.DependencyInjection;

public static class StrutSwashbuckleServiceCollectionExtensions
{
    public static IServiceCollection AddStrutSwashbuckle(this IServiceCollection services)
    {
        services.TryAddSingleton<TypeInfoCache>();

        services.AddOptions<SwaggerGenOptions>().Configure<TypeInfoCache>((o, cache) =>
        {
            o.SupportNonNullableReferenceTypes();
            o.ParameterFilter<RequiredParameterFilter>();
            o.SchemaFilter<RequiredPropertySchemaFilter>(cache);
            o.RequestBodyFilter<RequiredRequestBodyFilter>();
            o.UseAllOfToExtendReferenceSchemas();
        });

        return services;
    }
}

