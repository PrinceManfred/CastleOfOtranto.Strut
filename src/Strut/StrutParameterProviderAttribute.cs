using System;

namespace CastleOfOtranto.Strut;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Parameter,
                AllowMultiple = false)]
public class StrutParameterProviderAttribute : Attribute
{
    public Type ParameterProviderType { get; init; }

    public StrutParameterProviderAttribute(Type parameterProviderType)
    {
        ArgumentNullException.ThrowIfNull(parameterProviderType);
        if (!typeof(IParameterProvider).IsAssignableFrom(parameterProviderType))
        {
            throw new ArgumentException($"{nameof(parameterProviderType)} must implement IParameterProvider");
        }

        ParameterProviderType = parameterProviderType;
    }
}
