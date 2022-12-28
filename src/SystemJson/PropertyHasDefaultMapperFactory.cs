using System;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CastleOfOtranto.Strut.SystemJson;

public static class PropertyHasDefaultMapperFactory
{
	public static JsonSerializerOptions? GetMvcJsonSerializerOptions(
		this IServiceProvider serviceProvider)
	{
        using IServiceScope serviceScope = serviceProvider.CreateScope();
        var jsonOptions = serviceScope.ServiceProvider.GetService<IOptions<Microsoft.AspNetCore.Mvc.JsonOptions>>();

		return jsonOptions?.Value.JsonSerializerOptions;
    }

    public static JsonSerializerOptions? GetHttpJsonSerializerOptions(
        this IServiceProvider serviceProvider)
    {
        using IServiceScope serviceScope = serviceProvider.CreateScope();
        var jsonOptions = serviceScope.ServiceProvider.GetService<IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>>();

        return jsonOptions?.Value.SerializerOptions;
    }

    public static PropertyHasDefaultMapper CreateWithMvcOptions(IServiceProvider serviceProvider)
    {
        var jsonSerializerOptions = serviceProvider.GetMvcJsonSerializerOptions();
        return new PropertyHasDefaultMapper(jsonSerializerOptions);
    }

    public static PropertyHasDefaultMapper CreateWithHttpOptions(IServiceProvider serviceProvider)
    {
        var jsonSerializerOptions = serviceProvider.GetHttpJsonSerializerOptions();
        return new PropertyHasDefaultMapper(jsonSerializerOptions);
    }
}

