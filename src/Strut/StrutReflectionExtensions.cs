using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Metadata;

namespace CastleOfOtranto.Strut;

public static class StrutReflectionExtensions
{
    private static readonly NullabilityInfoContext _nullabilityInfoContext = new();

    #region ParameterInfo

    public static NullabilityState GetNullabilityState(this ParameterInfo parameterInfo)
    {
        var nullibilityInfo = _nullabilityInfoContext.Create(parameterInfo);
        return nullibilityInfo.WriteState;
    }

    public static TypeInfoCacheEntry CreateTypeInfoCacheEntry(this ParameterInfo parameterInfo)
    {
        return new TypeInfoCacheEntry(parameterInfo.HasDefaultValue, parameterInfo.GetNullabilityState());
    }

    #endregion

    #region PropertyInfo

    public static NullabilityState GetNullabilityState(this PropertyInfo propertyInfo)
    {
        var nullibilityInfo = _nullabilityInfoContext.Create(propertyInfo);
        return nullibilityInfo.WriteState;
    }

    public static Type? GetContainerType(this PropertyInfo propertyInfo)
    {
        return propertyInfo.ReflectedType ?? propertyInfo.DeclaringType;
    }

    public static bool HasDefault(this PropertyInfo propertyInfo,
        bool nonPublicGetter = true,
        bool nonPublicConstructor = false)
    {
        Type? containerType = propertyInfo.GetContainerType();

        if (containerType is null) return false;

        try
        {
            var instance = CreateParameterlessInstance(containerType, nonPublicConstructor);
            if (instance is null) return false;

            var getMethod = propertyInfo.GetGetMethod(nonPublicGetter);
            if (getMethod is null) return false;

            var value = getMethod.Invoke(instance, null);
            var defaultValue = GetDefaultValue(propertyInfo.PropertyType);

            if (value != defaultValue)
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    public static TypeInfoCacheEntry CreateTypeInfoCacheEntry(this PropertyInfo propertyInfo,
        bool nonPublicGetter = true,
        bool nonPublicConstructor = false)
    {
        TypeInfoCacheEntry entry = new(propertyInfo.HasDefault(nonPublicGetter, nonPublicConstructor),
            propertyInfo.GetNullabilityState());
           
        return entry;
    }

    #endregion

    #region FieldInfo

    public static NullabilityState GetNullabilityState(this FieldInfo fieldInfo)
    {
        var nullibilityInfo = _nullabilityInfoContext.Create(fieldInfo);
        return nullibilityInfo.WriteState;
    }

    public static Type? GetContainerType(this FieldInfo fieldInfo)
    {
        return fieldInfo.ReflectedType ?? fieldInfo.DeclaringType;
    }

    public static bool HasDefault(this FieldInfo fieldInfo,
        bool nonPublicGetter = true,
        bool nonPublicConstructor = false)
    {
        Type? containerType = fieldInfo.GetContainerType();

        if (containerType is null) return false;

        try
        {
            var instance = CreateParameterlessInstance(containerType, nonPublicConstructor);
            if (instance is null) return false;

            var value = fieldInfo.GetValue(instance);
            var defaultValue = GetDefaultValue(fieldInfo.FieldType);

            if (value != defaultValue)
            {
                return true;
            }
        }
        catch { }

        return false;
    }

    public static TypeInfoCacheEntry CreateTypeInfoCacheEntry(this FieldInfo fieldInfo,
        bool nonPublicGetter = true,
        bool nonPublicConstructor = false)
    {
        TypeInfoCacheEntry entry = new(fieldInfo.HasDefault(nonPublicGetter, nonPublicConstructor),
            fieldInfo.GetNullabilityState());

        return entry;
    }

    #endregion

    #region Private Methods

    private static object? CreateParameterlessInstance(Type type, bool nonPublic = false)
    {
        try
        {
            return Activator.CreateInstance(type, nonPublic);
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

    #endregion
}

