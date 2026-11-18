using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace ZLogger.MessagePack.Tests;

// The expected bytes were produced by ZLogger.MessagePack on master, which used MessagePack 2.5.187,
// so these tests pin the output of the current release byte for byte.
public class WireCompatibilityTest
{
    readonly FakeTimeProvider timeProvider = new(new DateTimeOffset(2026, 10, 23, 1, 2, 3, 456, TimeSpan.Zero).AddTicks(7));

    ILogger CreateLogger(TestProcessor processor)
    {
        var loggerFactory = LoggerFactory.Create(x =>
        {
            x.SetMinimumLevel(LogLevel.Debug);
            x.AddZLoggerLogProcessor(options =>
            {
                options.TimeProvider = timeProvider;
                options.IncludeScopes = true;
                return processor;
            });
        });
        return loggerFactory.CreateLogger("test");
    }

    /// <summary>
    /// Ensures that a log entry with the default properties, a scope and parameters of primitive, string, nullable, enum,
    /// decimal and array types is written exactly as the release that used MessagePack v2 wrote it.
    /// </summary>
    [Fact]
    public void EntryMatchesMessagePackV2Output()
    {
        const string expected =
            "DE0015A843617465676F7279A474657374A84C6F674C6576656CAB496E666F726D6174696F6EA954696D657374616D70D7FF6CB812F06ADAB20B" +
            "A74D657373616765D9682D3132333435203530303030303030303020323030202D31303020E38182205472756520312E3520322E323520E697A5" +
            "E69CACE8AA9EE381AEE38386E382ADE382B9E383882034322020205761726E696E67203132332E343536205B312C3230302C37303030305D" +
            "A2533107A25332A573636F7065A169D1CFC7A16CCF000000012A05F200A162CCC8A27362D09CA163CD3042A166C3A164CB3FF8000000000000" +
            "A2666CCA40100000A173B8E697A5E69CACE8AA9EE381AEE38386E382ADE382B9E38388A26E692AA26E6EC0A26E73C0A16503A16DA73132332E" +
            "343536A36172729301CCC8CE00011170";

        var processor = new TestProcessor(new MessagePackZLoggerFormatter());
        var logger = CreateLogger(processor);

        int i = -12345; long l = 5_000_000_000L; byte b = 200; sbyte sb = -100; char c = 'あ'; bool f = true;
        double d = 1.5; float fl = 2.25f; string s = "日本語のテキスト"; int? ni = 42; int? nn = null; string? ns = null;
        var e = LogLevel.Warning; var m = 123.456m; var arr = new[] { 1, 200, 70000 };
        using (logger.BeginScope("{S1} {S2}", 7, "scope"))
        {
            logger.ZLogInformation($"{i} {l} {b} {sb} {c} {f} {d} {fl} {s} {ni} {nn} {ns} {e} {m} {arr}");
        }

        Convert.ToHexString(processor.DequeueAsRaw()).Should().Be(expected);
    }

    public static TheoryData<string, object, string> SingleValueCases => new()
    {
        // The values MessagePack v2 wrote for these types (master wrote no value for DateTime and wrote the others twice,
        // so these come from MessagePackSerializer.Serialize of MessagePack 2.5.187).
        { "dt", new DateTime(2026, 10, 23, 1, 2, 3, 456, DateTimeKind.Utc).AddTicks(7), "D7FF6CB812F06ADAB20B" },
        { "dto", new DateTimeOffset(2026, 10, 23, 10, 2, 3, TimeSpan.FromHours(9)), "92D6FF6ADB309BCD021C" },
        { "ts", TimeSpan.FromMilliseconds(123456.789), "CE499602D2" },
        { "guid", new Guid("0f8fad5b-d9cb-469f-a165-70867728950e"), "D92430663866616435622D643963622D343639662D613136352D373038363737323839353065" },
    };

    /// <summary>
    /// Ensures that parameters of the types with dedicated branches are written once each, in the encoding that
    /// MessagePack v2 used for them with the default options.
    /// </summary>
    [Theory]
    [MemberData(nameof(SingleValueCases))]
    public void ValueMatchesMessagePackV2Output(string name, object value, string expectedValue)
    {
        var processor = new TestProcessor(new MessagePackZLoggerFormatter { IncludeProperties = IncludeProperties.ParameterKeyValues });
        var logger = CreateLogger(processor);

        switch (value)
        {
            case DateTime dt: logger.ZLogInformation($"{dt}"); break;
            case DateTimeOffset dto: logger.ZLogInformation($"{dto}"); break;
            case TimeSpan ts: logger.ZLogInformation($"{ts}"); break;
            case Guid guid: logger.ZLogInformation($"{guid}"); break;
        }

        // a map of one entry: the key as fixstr, then the value
        var expected = "81" + Convert.ToHexString([(byte)(0xa0 | name.Length)]) + Convert.ToHexString(System.Text.Encoding.UTF8.GetBytes(name)) + expectedValue;
        Convert.ToHexString(processor.DequeueAsRaw()).Should().Be(expected);
    }
}
