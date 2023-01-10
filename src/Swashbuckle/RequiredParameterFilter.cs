using System;
using System;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredParameterFilter : IParameterFilter
{
    public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
    {
        if (context.ParameterInfo.GetCustomAttributes()
            .OfType<StrutIgnoreAttribute>()
            .FirstOrDefault() is not null)
        {
            return;
        }

        if (context.PropertyInfo is not null)
        {
            parameter.Name = $"{context.ParameterInfo.Name}.{parameter.Name}";
        }

        if (parameter.Required) return;
        if (context.ParameterInfo.ParameterType.IsValueType) return;
        if (context.ParameterInfo.HasDefaultValue) return;
        
        if (context.PropertyInfo is not null)
        {
            if(context.PropertyInfo.SetMethod is null)
            {
                parameter.Extensions.Add(IsReadOnlyExtension.EXTENSION_NAME, IsReadOnlyExtension.Yes);
                return;
            }

            if (parameter.Schema.Extensions.ContainsKey(HasDefaultValueExtension.EXTENSION_NAME))
            {
                parameter.Schema.Extensions.Remove(HasDefaultValueExtension.EXTENSION_NAME);
            }

            if (context.PropertyInfo.HasDefault())
            {
                return;
            }

            if (context.PropertyInfo.GetNullabilityState() == NullabilityState.NotNull)
            {
                parameter.Required = true;
                
            }
            return;
        }

        if(context.ParameterInfo.GetNullabilityState() == NullabilityState.NotNull)
        {
            parameter.Required = true;
        }

        return;
    }
}

