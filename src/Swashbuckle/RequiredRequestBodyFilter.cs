using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class RequiredRequestBodyFilter : IRequestBodyFilter
{

    public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
    {
        requestBody.Required = true;
    }
}

