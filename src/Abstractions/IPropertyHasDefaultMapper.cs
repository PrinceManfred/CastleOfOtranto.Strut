using System;
using System.Collections.Generic;

namespace CastleOfOtranto.Strut.Abstractions;

public interface IPropertyHasDefaultMapper
{
    public IDictionary<string, bool>? GetPropertyHasDefaultMap(object instance
        , Type type);
}

