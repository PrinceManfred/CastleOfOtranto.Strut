using System;
namespace CastleOfOtranto.Strut;

public record class ParameterDescription(Type Type,
    string Name,
    bool IsRequired = false,
    bool OmitPrefix = false);
