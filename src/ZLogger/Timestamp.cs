using System.Buffers;
using System.Runtime.CompilerServices;
using Utf8StringInterpolation;

namespace ZLogger;

public readonly struct Timestamp
{
    readonly DateTimeOffset utcNow;

    // uses GetUtcNow() and LocalTimeZone
    readonly TimeProvider? timeProvider;

    public DateTimeOffset Utc => utcNow;

    public DateTimeOffset Local
    {
        get
        {
            if (timeProvider == null)
            {
                return utcNow.ToLocalTime();
            }
            else
            {
                return ToLocalTime(utcNow, timeProvider);
            }
        }
    }

    public override string ToString()
    {
        return Local.ToString();
    }

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

    // DateTimeOffset does not have LocalTime(TimeProvider) so implement myself.

    static readonly long MinTicks = DateTime.MinValue.Ticks;
    static readonly long MaxTicks = DateTime.MaxValue.Ticks;

    static DateTimeOffset ToLocalTime(DateTimeOffset dateTimeOffset, TimeProvider timeProvider)
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

    internal bool TryFormat(ref Utf8StringWriter<IBufferWriter<byte>> writer, string? format)
    {
        if (format == null)
        {
            // no format string, uses Local time
            FormatDateAndTimeAndMilliseconds(ref writer, Local.DateTime);
            return true;
        }

        switch (format)
        {
            case "local":
            case "local-longdate":
            case "longdate":
                FormatDateAndTimeAndMilliseconds(ref writer, Local.DateTime);
                return true;
            case "utc":
            case "utc-longdate":
                FormatDateAndTimeAndMilliseconds(ref writer, Utc.DateTime);
                return true;

            case "datetime":
            case "local-datetime":
                FormatDateAndTime(ref writer, Local.DateTime);
                return true;
            case "utc-datetime":
                FormatDateAndTime(ref writer, Utc.DateTime);
                return true;

            case "dateonly":
            case "local-dateonly":
                FormatDate(ref writer, Local.DateTime);
                return true;
            case "utc-dateonly":
                FormatDate(ref writer, Utc.DateTime);
                return true;

            case "timeonly":
            case "local-timeonly":
                FormatTime(ref writer, Local.DateTime);
                return true;
            case "utc-timeonly":
                FormatTime(ref writer, Utc.DateTime);
                return true;

            default:
                return false;
        }
    }

    static void FormatDate(ref Utf8StringWriter<IBufferWriter<byte>> writer, DateTime date)
    {
        // yyyy-
        var year = date.Year;
        if (year < 10) // 0~9
        {
            AppendWithFillZero3(ref writer, year);
        }
        else if (year < 100) // 10~99
        {
            AppendWithFillZero2(ref writer, year);
        }
        else if (year < 1000) // 100~999
        {
            AppendWithFillZero1(ref writer, year);
        }
        else // 1000~9999
        {
            writer.AppendFormatted(year);
        }
        writer.AppendUtf8("-"u8);

        // MM-
        var month = date.Month;
        if (month < 10)
        {
            AppendWithFillZero1(ref writer, month);
        }
        else
        {
            writer.AppendFormatted(month);
        }
        writer.AppendUtf8("-"u8);

        // 'dd'
        var day = date.Day;
        if (day < 10)
        {
            AppendWithFillZero1(ref writer, day);
        }
        else
        {
            writer.AppendFormatted(day);
        }
    }

    static void FormatTime(ref Utf8StringWriter<IBufferWriter<byte>> writer, DateTime time)
    {
        // HH:
        var hour = time.Hour;
        if (hour < 10)
        {
            AppendWithFillZero1(ref writer, hour);
        }
        else
        {
            writer.AppendFormatted(hour);
        }
        writer.AppendUtf8(":"u8);

        // mm:
        var minute = time.Minute;
        if (minute < 10)
        {
            AppendWithFillZero1(ref writer, minute);
        }
        else
        {
            writer.AppendFormatted(minute);
        }
        writer.AppendUtf8(":"u8);

        // ss
        var second = time.Second;
        if (second < 10)
        {
            AppendWithFillZero1(ref writer, second);
        }
        else
        {
            writer.AppendFormatted(second);
        }
    }

    static void FormatTimeMilliseconds(ref Utf8StringWriter<IBufferWriter<byte>> writer, DateTime dateTime)
    {
        var millisecond = dateTime.Millisecond; // The returned value is an integer between 0 and 999.
        if (millisecond < 10)
        {
            AppendWithFillZero2(ref writer, millisecond);
        }
        else if (millisecond < 100)
        {
            AppendWithFillZero1(ref writer, millisecond);
        }
        else
        {
            writer.AppendFormatted(millisecond);
        }
    }

    static void FormatDateAndTime(ref Utf8StringWriter<IBufferWriter<byte>> writer, DateTime dateTime)
    {
        // `yyyy-MM-dd HH:mm:ss`
        FormatDate(ref writer, dateTime);
        writer.AppendUtf8(" "u8);
        FormatTime(ref writer, dateTime);
    }

    static void FormatDateAndTimeAndMilliseconds(ref Utf8StringWriter<IBufferWriter<byte>> writer, DateTime dateTime)
    {
        // `yyyy-MM-dd HH:mm:ss.fff` (short form of ISO 8601)

        FormatDate(ref writer, dateTime);
        writer.AppendUtf8(" "u8);
        FormatTime(ref writer, dateTime);
        writer.AppendUtf8("."u8);
        FormatTimeMilliseconds(ref writer, dateTime);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AppendWithFillZero1(ref Utf8StringWriter<IBufferWriter<byte>> writer, int value)
    {
        writer.AppendUtf8("0"u8);
        writer.AppendFormatted(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AppendWithFillZero2(ref Utf8StringWriter<IBufferWriter<byte>> writer, int value)
    {
        writer.AppendUtf8("00"u8);
        writer.AppendFormatted(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AppendWithFillZero3(ref Utf8StringWriter<IBufferWriter<byte>> writer, int value)
    {
        writer.AppendUtf8("000"u8);
        writer.AppendFormatted(value);
    }
}
