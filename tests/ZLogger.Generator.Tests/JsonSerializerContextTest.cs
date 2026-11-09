#nullable enable

using System;
using System.Buffers;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZLogger.Generator.Tests;

public partial class JsonSerializerContextTest
{
    [ZLoggerMessage(LogLevel.Information, "{obj} {payload} {value} {items}", JsonSerializerContext = typeof(LogJsonContext))]
    static partial void Structured(ILogger logger, object? obj, ContextPayload? payload, ContextValue? value, int[]? items);

    [ZLoggerMessage(LogLevel.Information, "Before {payload:json} and {items} after", JsonSerializerContext = typeof(LogJsonContext))]
    static partial void Message(ILogger logger, ContextPayload? payload, ContextPayload[]? items);

    [Fact]
    public void StructuredParametersUseContext()
    {
        using var factory = TestHelper.CreateJsonLogger<JsonSerializerContextTest>(out var logger, out var messages);
        var payload = new ContextPayload { ValueName = "sample" };

        Structured(logger, payload, payload, new ContextValue { ValueCount = 42 }, new[] { 1, 2 });

        messages.Should().Equal("""
            {"obj":{"valueName":"sample"},"payload":{"valueName":"sample"},"value":{"valueCount":42},"items":[1,2]}
            """);
    }

    [Fact]
    public void StructuredParametersHandleNull()
    {
        using var factory = TestHelper.CreateJsonLogger<JsonSerializerContextTest>(out var logger, out var messages);

        Structured(logger, null, null, null, null);

        messages.Should().Equal("""
            {"obj":null,"payload":null,"value":null,"items":null}
            """);
    }

    [Theory]
    [InlineData(false, "Before {\"valueName\":\"sample\"} and [{\"valueName\":\"sample\"}] after")]
    [InlineData(true, "Before null and null after")]
    public void MessageFormattingUsesContext(bool useNull, string expected)
    {
        using var factory = TestHelper.CreateMessageLogger<JsonSerializerContextTest>(out var logger, out var messages);
        var payload = useNull ? null : new ContextPayload { ValueName = "sample" };

        Message(logger, payload, useNull ? null : new[] { payload! });

        messages.Should().Equal(expected);
    }

    [Fact]
    public void MissingMetadataThrows()
    {
        var state = new StructuredState(new UnregisteredPayload(), null!, null, null!);
        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        writer.WriteStartObject();

        Action serialize = () => state.WriteJsonParameterKeyValues(writer, new JsonSerializerOptions());

        serialize.Should().Throw<NotSupportedException>();
    }

    [Theory]
    [InlineData("string")]
    [InlineData("int[]")]
    public void InvalidContextReportsDiagnostic(string contextType)
    {
        var diagnostics = CSharpGeneratorRunner.RunGenerator($$"""
            using Microsoft.Extensions.Logging;
            using ZLogger;

            public static partial class Log
            {
                [ZLoggerMessage(LogLevel.Information, "{value}", JsonSerializerContext = typeof({{contextType}}))]
                public static partial void Write(ILogger logger, object value);
            }
            """);

        diagnostics.Where(x => x.Id != "CS8795").Select(x => x.Id).Should().Equal("ZLOG014");
    }

    sealed class UnregisteredPayload
    {
        public int Value { get; set; }
    }
}

public sealed class ContextPayload
{
    public string? ValueName { get; set; }
    public string? OptionalValue { get; set; }
}

public struct ContextValue
{
    public int ValueCount { get; set; }
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(ContextPayload))]
[JsonSerializable(typeof(ContextPayload[]))]
[JsonSerializable(typeof(ContextValue))]
[JsonSerializable(typeof(int[]))]
internal partial class LogJsonContext : JsonSerializerContext
{
}
