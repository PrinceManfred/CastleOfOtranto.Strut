using System;
using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredPropertySchemaFilter : ISchemaFilter
{
    private NullabilityInfoContext _nullabilityInfoContext = new();

    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, bool>>
        _cache = new();

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.MemberInfo is not null)
        {
            if (context.MemberInfo is PropertyInfo propertyInfo)
            {
                ProcessProperty(propertyInfo, schema);
            }
            else if(context.MemberInfo is FieldInfo fieldInfo)
            {
                ProcessField(fieldInfo, schema);
            }
        }

        foreach(var prop in schema.Properties)
        {
            if(prop.Value.Extensions.ContainsKey(IsDefaultValueExtension.EXTENSION_NAME))
            {
                IOpenApiExtension isDefault = prop.Value.Extensions[IsDefaultValueExtension.EXTENSION_NAME];
                prop.Value.Extensions.Remove(IsDefaultValueExtension.EXTENSION_NAME);

                if (!prop.Value.Nullable && isDefault == IsDefaultValueExtension.Yes)
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
        if (propertyInfo.ReflectedType is null) return;

        ConcurrentDictionary<string, bool> defaultMap = GetDefaultMap(propertyInfo.ReflectedType);
        if (defaultMap.TryGetValue(propertyInfo.Name, out bool isDefault))
        {
            if (isDefault)
            {
                schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.No);
            }
        }
        else
        {
            try
            {
                var instance = CreateParameterlessInstance(propertyInfo.ReflectedType);
                if (instance is null) return;

                var getMethod = propertyInfo.GetGetMethod(true);

                if (getMethod is null) return;

                var value = getMethod.Invoke(instance, null);
                var defaultValue = propertyInfo.PropertyType.GetDefaultValue();

                if (value == defaultValue || (value is not null && value.Equals(defaultValue)))
                {
                    defaultMap.TryAdd(propertyInfo.Name, true);
                    schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.Yes);
                }
                else
                {
                    defaultMap.TryAdd(propertyInfo.Name, false);
                    schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.No);
                }
            }
            catch
            {
                return;
            }
        }
    }

    private void ProcessField(FieldInfo fieldInfo, OpenApiSchema schema)
    {
        if (fieldInfo.FieldType.IsValueType) return;
        if (fieldInfo.ReflectedType is null) return;

        ConcurrentDictionary<string, bool> defaultMap = GetDefaultMap(fieldInfo.ReflectedType);
        if (defaultMap.TryGetValue(fieldInfo.Name, out bool isDefault))
        {
            if (isDefault)
            {
                schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.Yes);
            }
            else
            {
                schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.No);
            }

        }
        else
        {
            try
            {
                var instance = CreateParameterlessInstance(fieldInfo.ReflectedType);
                if (instance is null) return;

                var value = fieldInfo.GetValue(instance);
                var defaultValue = fieldInfo.FieldType.GetDefaultValue();

                if (value == defaultValue || (value is not null && value.Equals(defaultValue)))
                {
                    defaultMap.TryAdd(fieldInfo.Name, true);
                    schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.Yes);
                }
                else
                {
                    defaultMap.TryAdd(fieldInfo.Name, false);
                    schema.Extensions.TryAdd(IsDefaultValueExtension.EXTENSION_NAME, IsDefaultValueExtension.No);
                }
            }
            catch
            {
                return;
            }
        }
    }

    private ConcurrentDictionary<string, bool> GetDefaultMap(Type type)
    {
        return _cache.GetOrAdd(type, new ConcurrentDictionary<string, bool>());
    }
}

