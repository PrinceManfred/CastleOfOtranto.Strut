using System;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public static class StrutSwaggerGenOptionsExtensions
{
    public static void ApplyStrut(this SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.SupportNonNullableReferenceTypes();
        swaggerGenOptions.ParameterFilter<RequiredParameterFilter>();
        swaggerGenOptions.SchemaFilter<RequiredPropertySchemaFilter>();
        swaggerGenOptions.UseAllOfToExtendReferenceSchemas();
    }
}

