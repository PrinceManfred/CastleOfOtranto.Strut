using System;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class HasDefaultValueExtension : IOpenApiExtension
{
    public static readonly HasDefaultValueExtension Yes = new(YES_DEFAULT);
    public static readonly HasDefaultValueExtension No = new(NO_DEFAULT);

    private const string YES_DEFAULT = "yes";
    private const string NO_DEFAULT = "no";

    private readonly string _hasDefault;

    public const string EXTENSION_NAME = "x-castleofotranto-has-default";

    private HasDefaultValueExtension(string hasDefault)
    {
        _hasDefault = hasDefault;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteValue(_hasDefault);
    }
}

