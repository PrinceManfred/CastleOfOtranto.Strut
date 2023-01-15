using System;

namespace CastleOfOtranto.Strut;

[AttributeUsage(AttributeTargets.Field |
                AttributeTargets.Property |
                AttributeTargets.Parameter,
                AllowMultiple = false)]
public class StrutIgnoreAttribute : Attribute { }

