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
    readonly DateTimeOffset utcNow;
#if NET8_0_OR_GREATER
    // uses GetUtcNow() and LocalTimeZone
    readonly TimeProvider? timeProvider;
#endif

    public DateTimeOffset Utc => utcNow;
    public DateTimeOffset Local
    {
        get
        {
#if NET8_0_OR_GREATER
            if (timeProvider == null)
            {
                return utcNow.ToLocalTime();
            }
            else
            {
                return ToLocalTime(utcNow, timeProvider);
            }
#else
            return utcNow.ToLocalTime();
#endif
        }
    }

    public override string ToString()
    {
        return Local.ToString();
    }

#if NET8_0_OR_GREATER

    static readonly long MinTicks = DateTime.MinValue.Ticks;
    static readonly long MaxTicks = DateTime.MaxValue.Ticks;

    public Timestamp(TimeProvider? timeProvider)
    {
        if (timeProvider == null)
        {
            this.utcNow = DateTimeOffset.UtcNow;
            this.timeProvider = null;
        }
        else
        {
            this.utcNow = timeProvider.GetUtcNow();
            this.timeProvider = timeProvider;
        }
    }

    private static DateTimeOffset ToLocalTime(DateTimeOffset dateTimeOffset, TimeProvider timeProvider)
    {
        var utcDateTime = dateTimeOffset.UtcDateTime;
        var offset = timeProvider.LocalTimeZone.GetUtcOffset(utcDateTime);
        var localTicks = utcDateTime.Ticks + offset.Ticks;

        if (localTicks < MinTicks || localTicks > MaxTicks)
        {
            localTicks = localTicks < MinTicks ? MinTicks : MaxTicks;
        }

        return new DateTimeOffset(localTicks, offset);
    }
#else
    
    public Timestamp(DateTimeOffset utcNow)
    {
        this.utcNow = utcNow;
    }

#endif
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