using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ZLogger;

public readonly struct LogInfo
{
    public readonly LogCategory Category;
    public readonly Timestamp Timestamp;
    public readonly LogLevel LogLevel;
    public readonly EventId EventId;
    public readonly Exception? Exception;

    public LogInfo(LogCategory category, Timestamp timestamp, LogLevel logLevel, EventId eventId, Exception? exception)
    {
        Category = category;
        Timestamp = timestamp;
        EventId = eventId;
        LogLevel = logLevel;
        Exception = exception;
    }
}

public readonly struct Timestamp
{
    public static Timestamp Now => new Timestamp(DateTimeOffset.UtcNow);

    readonly DateTimeOffset utcNow;

    public DateTimeOffset Utc => utcNow;
    public DateTimeOffset Local => utcNow.ToLocalTime();

    Timestamp(DateTimeOffset utcNow)
    {
        this.utcNow = utcNow;
    }

    public override string ToString()
    {
        return Local.ToString();
    }
}

public readonly struct LogCategory
{
    readonly byte[] utf8;
    public readonly string Name;
    public readonly JsonEncodedText JsonEncoded;
    public readonly ReadOnlySpan<byte> Utf8Span => utf8;
    public readonly ReadOnlyMemory<byte> Utf8Memory => utf8;


    public LogCategory(string category)
    {
        this.Name = category;
        this.utf8 = Encoding.UTF8.GetBytes(category);
        this.JsonEncoded = JsonEncodedText.Encode(utf8);
    }

    public override string ToString()
    {
        return Name;
    }
}