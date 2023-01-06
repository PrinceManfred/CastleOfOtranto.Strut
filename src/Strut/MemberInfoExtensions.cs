using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Metadata;

namespace CastleOfOtranto.Strut;

public static class MemberInfoExtensions
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();

    public static TypeInfoCacheEntry CreateTypeInfoCacheEntry(this ParameterInfo parameterInfo)
    {
        var isDefault = !parameterInfo.HasDefaultValue;
        var nullibilityInfo = _nullabilityInfoContext.Create(parameterInfo);

        return new TypeInfoCacheEntry(isDefault, nullibilityInfo.WriteState);
    }

    public static bool TryCreateTypeInfoCacheEntry(this PropertyInfo propertyInfo, out TypeInfoCacheEntry entry, bool nonPublic = true)
    {
        entry = default;
        Type? containingType = propertyInfo.ReflectedType ?? propertyInfo.DeclaringType;

        if (containingType is null) return false;

        try
        {
            var instance = CreateParameterlessInstance(containingType);
            if (instance is null) return false;

            var getMethod = propertyInfo.GetGetMethod(nonPublic);
            if (getMethod is null) return false;

            var value = getMethod.Invoke(instance, null);
            var defaultValue = GetDefaultValue(propertyInfo.PropertyType);

            bool isDefault;

            if (value == defaultValue)
            {
                isDefault = true;
            }
            else
            {
                isDefault = false;
            }

            var nullibilityInfo = _nullabilityInfoContext.Create(propertyInfo);

            entry = new(isDefault, nullibilityInfo.WriteState);
            return true;
        }
        catch { }

        return false;
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

    private static object? GetDefaultValue(Type type)
    {
        if (!type.IsValueType)
        {
            return null;
        }
        return Activator.CreateInstance(type);
    }
}

