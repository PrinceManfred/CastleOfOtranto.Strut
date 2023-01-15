using System;
using Microsoft.Extensions.DependencyInjection;

namespace CastleOfOtranto.Strut;

public static class StrutParameterProviderUtilities
{
    public static IParameterProvider Create<T>() where T : IParameterProvider
    {
        return Activator.CreateInstance<T>();
    }

    public static IParameterProvider? Create(Type type)
    {
        if (!typeof(IParameterProvider).IsAssignableFrom(type))
        {
            throw new ArgumentException($"{nameof(type)} must implement IParameterProvider");
        }

        return Activator.CreateInstance(type) as IParameterProvider;
    }

    public static IParameterProvider Create<T>(IServiceProvider provider) where T : IParameterProvider
    {
        return ActivatorUtilities.GetServiceOrCreateInstance<T>(provider);
    }

    public static IParameterProvider? Create(Type type, IServiceProvider provider)
    {
        if (!typeof(IParameterProvider).IsAssignableFrom(type))
        {
            throw new ArgumentException($"{nameof(type)} must implement IParameterProvider");
        }

        return ActivatorUtilities.GetServiceOrCreateInstance(provider, type) as IParameterProvider;
    }
}

