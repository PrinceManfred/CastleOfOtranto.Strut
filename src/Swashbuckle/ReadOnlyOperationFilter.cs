using System;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CastleOfOtranto.Strut.Swashbuckle
{
	public class ReadOnlyOperationFilter : IOperationFilter
	{
		public ReadOnlyOperationFilter()
		{
		}

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters = operation.Parameters.Where(p =>
            {
                if(p.Extensions.TryGetValue(IsReadOnlyExtension.EXTENSION_NAME, out var isReadOnly))
                {
                    p.Extensions.Remove(IsReadOnlyExtension.EXTENSION_NAME);
                    if(isReadOnly == IsReadOnlyExtension.Yes) return false;
                }

                return true;
            })
            .ToList();
            

        }
    }
}

