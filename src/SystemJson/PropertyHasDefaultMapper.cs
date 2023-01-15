using System;
using System.Collections.Generic;
using System.Text.Json;
using CastleOfOtranto.Strut.Abstractions;

namespace CastleOfOtranto.Strut.SystemJson;

public class PropertyHasDefaultMapper : IPropertyHasDefaultMapper
{
    private readonly JsonSerializerOptions? _options;

    public PropertyHasDefaultMapper(JsonSerializerOptions? options = null)
    {
        _options = options;
    }

    public IDictionary<string, bool>? GetPropertyHasDefaultMap(object instance
        , Type type)
    {
        Dictionary<string, bool>? propDefaultMap;

        try
        {
            JsonElement instanceElement = JsonSerializer.SerializeToElement(instance
                                                                , type
                                                                , _options);

            if (instanceElement.ValueKind != JsonValueKind.Object) return null;

            propDefaultMap = new();

            foreach (var jsonProp in instanceElement.EnumerateObject())
            {
                bool hasDefault = true;

                switch (jsonProp.Value.ValueKind)
                {
                    // These cases represent all values that succesfully
                    // bind to a non-nullable property. We'll treat them
                    // as if they are default assignments due to lack of
                    // more runtime information.
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                    case JsonValueKind.Number:
                    case JsonValueKind.False:
                    case JsonValueKind.True:
                    case JsonValueKind.String:
                        break;

                    case JsonValueKind.Undefined:
                    case JsonValueKind.Null:
                    default:
                        hasDefault = false;
                        break;
                }

                propDefaultMap.Add(jsonProp.Name, hasDefault);
            }
        }
        catch
        {
            // Something's gone oddly wrong.
            return null;
        }

        return propDefaultMap;
    }
}

