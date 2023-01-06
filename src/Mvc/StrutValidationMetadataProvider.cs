using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CastleOfOtranto.Strut.Mvc
{
	public class StrutValidationMetadataProvider : IValidationMetadataProvider
    {
        private readonly TypeInfoCache _cache;
        private static readonly NullabilityInfoContext _context = new();

        public StrutValidationMetadataProvider(TypeInfoCache cache)
        {
            _cache = cache;
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context.Key.ParameterInfo is not null)
            {
                ProcessParameter(context.ValidationMetadata, context.Key.ParameterInfo);
                return;
            }

            if(context.Key.PropertyInfo is not null)
            {
                ProcessProperty(context.ValidationMetadata, context.Key.PropertyInfo);
                return;
            }
            
        }

        private void ProcessParameter(ValidationMetadata validationMetadata, ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsValueType) return;
            if (parameterInfo.HasDefaultValue) return;

            NullabilityInfo info = _context.Create(parameterInfo);
            if (info.WriteState == NullabilityState.NotNull)
            {
                var requiredAttribute = parameterInfo.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute is not null) return;

                requiredAttribute = new RequiredAttribute
                {
                    AllowEmptyStrings = true
                };

                validationMetadata.IsRequired = true;

                if (!validationMetadata.ValidatorMetadata.Contains(requiredAttribute))
                {
                    validationMetadata.ValidatorMetadata.Add(requiredAttribute);
                }
            }
        }

        private void ProcessProperty(ValidationMetadata validationMetadata, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType.IsValueType) return;

            var containingType = propertyInfo.ReflectedType ?? propertyInfo.DeclaringType;
            if (containingType is null) return;

            TypeInfoCacheEntry entry;
            if(!_cache.TryGet(containingType, propertyInfo.Name, out entry))
            {
                if (!propertyInfo.TryCreateTypeInfoCacheEntry(out entry)) return;
                _cache.TryAdd(containingType, propertyInfo.Name, entry);
            }

            if (!entry.IsDefault) return;
            if (entry.NullabilityState == NullabilityState.NotNull)
            {
                var requiredAttribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();

                if (requiredAttribute is not null) return;

                requiredAttribute = new RequiredAttribute
                {
                    AllowEmptyStrings = true
                };

                validationMetadata.IsRequired = true;

                if (!validationMetadata.ValidatorMetadata.Contains(requiredAttribute))
                {
                    validationMetadata.ValidatorMetadata.Add(requiredAttribute);
                }
            }
        }
    }
}

