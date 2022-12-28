using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace CastleOfOtranto.Strut.Mvc;

public class MvcApiRequiredParameters : IApiDescriptionProvider
{
    public int Order { get; set; } = 1000;

    public void OnProvidersExecuting(ApiDescriptionProviderContext context)
    {
        // Do nothing. We want to give someone else a chance to do the hard
        // work of building the descriptors initially.
    }

    public void OnProvidersExecuted(ApiDescriptionProviderContext context)
    {
        foreach(var result in context.Results)
        {
            // We only care about MVC controller actions.
            if(result.ActionDescriptor is not ControllerActionDescriptor)
            {
                continue;
            }

            foreach(var param in result.ParameterDescriptions) {
                ProcessParameter(param);
            }
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