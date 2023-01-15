using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CastleOfOtranto.Strut.Mvc;

public class RequiredParametersApiDescriptionProvider : IApiDescriptionProvider
{
    private readonly HashSet<string> _processed = new();
    private readonly IServiceProvider _provider;
    private readonly IModelMetadataProvider _metadataProvider;

    public int Order { get; set; } = 1000;

    public RequiredParametersApiDescriptionProvider(IServiceProvider provider, IModelMetadataProvider metadataProvider)
    {
        _provider = provider;
        _metadataProvider = metadataProvider;
    }

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        // Do nothing. We want to give someone else a chance to do the hard
        // work of building the descriptors initially.
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        foreach (var result in context.Results)
        {
            // We only care about MVC controller actions.
            if (result.ActionDescriptor is not ControllerActionDescriptor)
            {
                continue;
            }

            if (result.ParameterDescriptions.Count == 0) continue;

            List<ApiParameterDescription> processedParameters = new();

            foreach (var param in result.ParameterDescriptions)
            {

                // TODO: Check parameter for StrutPArameterProviderAttribute first
                if (TryProcessParameterLevelParameterProvider(processedParameters, param))
                {
                    continue;
                }

                if (param.ParameterDescriptor?.BindingInfo?.BinderType is not null)
                {
                    ProcessTopLevelModelBinder(processedParameters, param);
                    continue;
                }

                if (param.ParameterDescriptor?.ParameterType != param.ModelMetadata.ModelType)
                {
                    if (param.BindingInfo?.BinderType is not null)
                    {
                        ProcessNestedModelBinder(processedParameters, param);
                        continue;
                    }

                    param.Name = $"{param.ParameterDescriptor?.BindingInfo?.BinderModelName ?? param.ParameterDescriptor?.Name}.{param.Name}";
                }

                // TODO: If no IParameterProviders on the ModelBinder check the Model class itself

                ProcessParameter(param);
                processedParameters.Add(param);
            }

            result.ParameterDescriptions.Clear();
            foreach (var processedParam in processedParameters)
            {
                result.ParameterDescriptions.Add(processedParam);
            }

        }
    }

    private bool TryProcessParameterLevelParameterProvider(List<ApiParameterDescription> parameters, ApiParameterDescription description)
    {
        var parameterDescriptor = description.ParameterDescriptor;
        if (parameterDescriptor is null) return false;
        if (_processed.Contains(parameterDescriptor.Name)) return true;

        var controllerParameterDescriptor = parameterDescriptor as ControllerParameterDescriptor;
        if (controllerParameterDescriptor is null) return false;

        var strutParameterProviderAttribute = controllerParameterDescriptor
                        .ParameterInfo
                        .GetCustomAttributes()
                        .OfType<StrutParameterProviderAttribute>()
                        .FirstOrDefault();

        if (strutParameterProviderAttribute is null) return false;

        AddCustomParameters(parameters, strutParameterProviderAttribute.ParameterProviderType, description, parameterDescriptor);

        return true;
    }

    private void ProcessTopLevelModelBinder(List<ApiParameterDescription> parameters, ApiParameterDescription description)
    {
        var parameterDescriptor = description.ParameterDescriptor;

        if (_processed.Contains(parameterDescriptor.Name)) return;

        var binderType = parameterDescriptor.BindingInfo?.BinderType;
        if (binderType is null) return;

        var attr = binderType.GetCustomAttributes(true)
            .OfType<StrutParameterProviderAttribute>()
            .FirstOrDefault();

        // TODO: Also check if the model binder implements IParameterProvider
        if (attr is null) return;

        AddCustomParameters(parameters, attr.ParameterProviderType, description, parameterDescriptor);
    }

    private void ProcessNestedModelBinder(List<ApiParameterDescription> parameters, ApiParameterDescription description)
    {
        var parameterDescriptor = description.ParameterDescriptor;

        var binderType = description.BindingInfo?.BinderType;
        if (binderType is null) return;

        var attr = binderType.GetCustomAttributes(true)
            .OfType<StrutParameterProviderAttribute>()
            .FirstOrDefault();

        // TODO: Also check if the model binder implements IParameterProvider
        if (attr is null) return;

        AddCustomParameters(parameters, attr.ParameterProviderType, description, parameterDescriptor);        
    }

    private void AddCustomParameters(List<ApiParameterDescription> parameters,
        Type parameterProviderType,
        ApiParameterDescription description,
        ParameterDescriptor parameterDescriptor)
    {
        var instance = StrutParameterProviderUtilities.Create(parameterProviderType, _provider);
        if (instance is null) return;

        foreach (ParameterDescription paramInfo in instance.GetParameters())
        {
            var data = _metadataProvider.GetMetadataForType(paramInfo.Type);
            var desc = new StrutApiParameterDescription();

            desc.BindingInfo = new BindingInfo
            {
                BinderModelName = data.BinderModelName,
                BinderType = data.BinderType,
                BindingSource = data.BindingSource,
                PropertyFilterProvider = data.PropertyFilterProvider
            };
            desc.ModelMetadata = data;

            if (paramInfo.OmitPrefix)
            {
                desc.Name = paramInfo.Name;
            }
            else
            {
                desc.Name = $"{parameterDescriptor.BindingInfo?.BinderModelName ?? parameterDescriptor.Name}.{description.Name}.{paramInfo.Name}";
            }

            desc.ParameterDescriptor = parameterDescriptor;
            desc.RouteInfo = description.RouteInfo;
            desc.Source = BindingSource.Custom;
            desc.Type = paramInfo.Type;
            desc.StrutIsRequired = paramInfo.IsRequired;
            parameters.Add(desc);
        }
    }

    private static void ProcessParameter(ApiParameterDescription description)
    {
        // MVC will always accept value types unless BindRequiredAttribute is set.
        if (!description.ModelMetadata.IsReferenceOrNullableType) return;

        // Someone else already figured this out so return.
        if (description.IsRequired) return;

        // MVC gets everything else right. They just don't surface it to
        // the API Explorer for some reason. Do that.
        description.IsRequired = description.ModelMetadata.IsRequired;
    }
}