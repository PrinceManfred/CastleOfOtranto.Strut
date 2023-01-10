using System;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Writers;

namespace CastleOfOtranto.Strut.Swashbuckle;

public class IsReadOnlyExtension : IOpenApiExtension
{
    public static readonly IsReadOnlyExtension Yes = new(YES_DEFAULT);
    public static readonly IsReadOnlyExtension No = new(NO_DEFAULT);

    private const string YES_DEFAULT = "yes";
    private const string NO_DEFAULT = "no";

    private readonly string _isReadonly;

    public const string EXTENSION_NAME = "x-castleofotranto-is-readonly";

    private IsReadOnlyExtension(string isReadonly)
    {
        _isReadonly = isReadonly;
    }

    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
    {
        writer.WriteValue(_isReadonly);
    }
}

