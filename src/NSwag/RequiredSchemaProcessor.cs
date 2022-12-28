using System;
using CastleOfOtranto.Strut.Abstractions;
using NJsonSchema.Generation;

namespace CastleOfOtranto.Strut.NSwag;

public class RequiredSchemaProcessor : ISchemaProcessor
{
    private readonly IPropertyHasDefaultMapper _propertyDefaultMapper;

    public RequiredSchemaProcessor(IPropertyHasDefaultMapper propertyDefaultMapper)
	{
        _propertyDefaultMapper = propertyDefaultMapper;
	}

    public void Process(SchemaProcessorContext context)
    {
        if (context.Schema.Properties.Count == 0) return;

        object? instance = CreateParameterlessInstance(context.ContextualType);
            
        // We can't make an informed decision without an instance.
        // User needs to provide some other means of annotating required
        // properties.
        if(instance is null) return;

        Dictionary<string, bool>? propDefaultMap =
            _propertyDefaultMapper.GetPropertyHasDefaultMap(instance, context.ContextualType, );

        if (propDefaultMap is null) return;
            
        foreach (var schemaProp in context.Schema.Properties)
        {
            // Bail out is someone else determined this is required.
            if (context.Schema.RequiredProperties.Contains(schemaProp.Key)) {
                continue;
            }

            // If not nullable and no default this prop is required.
            if (schemaProp.Value.IsNullableRaw != true
                && !propDefaultMap[schemaProp.Key])
            {
                context.Schema.RequiredProperties.Add(schemaProp.Key);
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
}

