using System;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class IsDefaultValueExtension : IOpenApiExtension
{
    public static readonly IsDefaultValueExtension Yes = new(YES_DEFAULT);
    public static readonly IsDefaultValueExtension No = new(NO_DEFAULT);

    private const string YES_DEFAULT = "yes";
    private const string NO_DEFAULT = "no";

    private readonly string _isDefault;

    public const string EXTENSION_NAME = "x-castleofotranto-is-default";

    private IsDefaultValueExtension(string isDefault)
    {
        _isDefault = isDefault;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteValue(_isDefault);
    }
}

