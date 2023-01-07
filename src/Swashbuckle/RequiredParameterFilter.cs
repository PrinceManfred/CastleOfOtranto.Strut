using System;
using System;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (parameter.Required) return;
        if (context.ParameterInfo.ParameterType.IsValueType) return;
        if (context.ParameterInfo.HasDefaultValue) return;

        if(context.PropertyInfo is not null)
        {
            if (context.PropertyInfo.HasDefault()) return;
            if (context.PropertyInfo.GetNullabilityState() != NullabilityState.NotNull)
            {
                return;
            }
        }

        if(context.ParameterInfo.GetNullabilityState() == NullabilityState.NotNull)
        {
            parameter.Required = true;
        }

        return;
    }
}

