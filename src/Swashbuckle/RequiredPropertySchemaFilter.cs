using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredPropertySchemaFilter : ISchemaFilter
{
    private readonly TypeInfoCache _cache;

    public RequiredPropertySchemaFilter(TypeInfoCache cache)
    {
        _cache = cache;
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.MemberInfo is not null)
        {
            if (context.MemberInfo.GetCustomAttributes()
                .OfType<StrutIgnoreAttribute>()
                .FirstOrDefault() is not null)
            {
                return;
            }

            if (context.MemberInfo is PropertyInfo propertyInfo)
            {
                ProcessProperty(propertyInfo, schema);
            }
            else if (context.MemberInfo is FieldInfo fieldInfo)
            {
                ProcessField(fieldInfo, schema);
            }
        }

        foreach (var prop in schema.Properties)
        {
            if (prop.Value.Extensions.ContainsKey(HasDefaultValueExtension.EXTENSION_NAME))
            {
                IOpenApiExtension hasDefault = prop.Value.Extensions[HasDefaultValueExtension.EXTENSION_NAME];
                prop.Value.Extensions.Remove(HasDefaultValueExtension.EXTENSION_NAME);

                if (!prop.Value.Nullable && hasDefault == HasDefaultValueExtension.No)
                {
                    schema.Required.Add(prop.Key);
                }
            }
        }
    }

    private static object? CreateParameterlessInstance(Type type)
    {
        try
        {
            // Try and create an instance with empty constructor so we
            // can see if the properties have initial assignments.
            return Activator.CreateInstance(type);
        }
        catch
        {
            // Swallow errors.
        }

        return null;
    }

    private void ProcessProperty(PropertyInfo propertyInfo, OpenApiSchema schema)
    {
        if (propertyInfo.PropertyType.IsValueType) return;

        var containerType = propertyInfo.GetContainerType();

        if (containerType is null) return;

        if (_cache.TryGet(containerType, propertyInfo.Name, out var entry))
        {
            if (entry.HasDefault)
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.No);
            }
        }
        else
        {
            var newEntry = propertyInfo.CreateTypeInfoCacheEntry();
            _cache.TryAdd(containerType, propertyInfo.Name, newEntry);

            if (newEntry.HasDefault)
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.No);
            }
        }
    }

    private void ProcessField(FieldInfo fieldInfo, OpenApiSchema schema)
    {
        if (fieldInfo.FieldType.IsValueType) return;

        var containerType = fieldInfo.GetContainerType();
        if (containerType is null) return;

        if (_cache.TryGet(containerType, fieldInfo.Name, out var entry))
        {
            if (entry.HasDefault)
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.No);
            }
        }
        else
        {
            var newEntry = fieldInfo.CreateTypeInfoCacheEntry();
            _cache.TryAdd(containerType, fieldInfo.Name, newEntry);

            if (newEntry.HasDefault)
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(HasDefaultValueExtension.EXTENSION_NAME, HasDefaultValueExtension.No);
            }
        }
    }
}

