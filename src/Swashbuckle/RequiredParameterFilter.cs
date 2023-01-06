using System;
using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredParameterFilter : IParameterFilter
{
    private readonly NullabilityInfoContext _nullabilityInfoContext = new();

    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.Required) return;
        if (context.ParameterInfo.ParameterType.IsValueType) return;
        if (context.ParameterInfo.HasDefaultValue) return;

        // TODO: context.PropertyInfo might not be null here if binding complex types from query
        // we need to process that if it's here.

        NullabilityInfo nullabilityInfo = _nullabilityInfoContext.Create(context.ParameterInfo);
        if(nullabilityInfo.WriteState == NullabilityState.NotNull)
        {
            parameter.Required = true;
        }

        return;
    }
}

