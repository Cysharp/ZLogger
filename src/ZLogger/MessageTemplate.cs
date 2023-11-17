using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Utf8StringInterpolation;

namespace ZLogger;

[InterpolatedStringHandler]
public struct MessageTemplateHandler(int literalLength, int formattedCount)
{
    readonly List<MessageTemplateChunk> templateChunk = new();

    public void AppendLiteral(string s)
    {
        templateChunk.Add(new(Encoding.UTF8.GetBytes(s), -1, 0, null, true));
    }

    public void AppendFormatted(int index, int alignment = 0, string? format = null)
    {
        if (index < 0) throw new ArgumentException("Index (zero based) must be greater than or equal to zero.");
        templateChunk.Add(new(null, index, alignment, format, (alignment == 0 && format == null)));
    }

    internal MessageTemplateHolder GetTemplate()
    {
        return new MessageTemplateHolder(literalLength, formattedCount, templateChunk.ToArray());
    }
}

internal sealed record MessageTemplateHolder(int LiteralLength, int FormattedCount, MessageTemplateChunk[] TemplateChunk);
internal readonly record struct MessageTemplateChunk(byte[]? Literal, int Index, int Alignment, string? Format, bool NoAlignmentAndFormat);

public readonly partial struct MessageTemplate
{
    readonly MessageTemplateHolder template;
    readonly IBufferWriter<byte> writer;

    internal MessageTemplate(MessageTemplateHolder template, IBufferWriter<byte> writer)
    {
        this.template = template;
        this.writer = writer;
    }

    public void Format()
    {
        var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
        }

        stringWriter.Flush();
    }

    static void AppendLogLevel(ref Utf8StringWriter<IBufferWriter<byte>> writer, ref LogLevel value, ref MessageTemplateChunk chunk)
    {
        if (!chunk.NoAlignmentAndFormat)
        {
            if (chunk is { Alignment: 0, Format: "short" })
            {
                switch (value)
                {
                    case LogLevel.Trace:
                        writer.AppendUtf8("TRC"u8);
                        return;
                    case LogLevel.Debug:
                        writer.AppendUtf8("DBG"u8);
                        return;
                    case LogLevel.Information:
                        writer.AppendUtf8("INF"u8);
                        return;
                    case LogLevel.Warning:
                        writer.AppendUtf8("WRN"u8);
                        return;
                    case LogLevel.Error:
                        writer.AppendUtf8("ERR"u8);
                        return;
                    case LogLevel.Critical:
                        writer.AppendUtf8("CRI"u8);
                        return;
                    case LogLevel.None:
                        writer.AppendUtf8("NON"u8);
                        return;
                    default:
                        break;
                }
            }

            writer.AppendFormatted(value, chunk.Alignment, chunk.Format);
            return;
        }

        switch (value)
        {
            case LogLevel.Trace:
                writer.AppendUtf8("Trace"u8);
                break;
            case LogLevel.Debug:
                writer.AppendUtf8("Debug"u8);
                break;
            case LogLevel.Information:
                writer.AppendUtf8("Information"u8);
                break;
            case LogLevel.Warning:
                writer.AppendUtf8("Warning"u8);
                break;
            case LogLevel.Error:
                writer.AppendUtf8("Error"u8);
                break;
            case LogLevel.Critical:
                writer.AppendUtf8("Critical"u8);
                break;
            case LogLevel.None:
                writer.AppendUtf8("None"u8);
                break;
            default:
                writer.AppendFormatted(value);
                break;
        }
    }

    static void AppendLogCategory(ref Utf8StringWriter<IBufferWriter<byte>> writer, ref LogCategory value, ref MessageTemplateChunk chunk)
    {
        if (!chunk.NoAlignmentAndFormat)
        {
            // currently ReadOnlySpan<byte> no support alignment/format.
            writer.AppendFormatted(value.Name, chunk.Alignment, chunk.Format);
            return;
        }

        writer.AppendUtf8(value.Utf8Span);
    }

    static void AppendTimestamp(ref Utf8StringWriter<IBufferWriter<byte>> writer, ref Timestamp value, ref MessageTemplateChunk chunk)
    {
        if (!chunk.NoAlignmentAndFormat)
        {
            // uses Local DateTimeOffset
            writer.AppendFormatted(value.Local, chunk.Alignment, chunk.Format);
            return;
        }

        // no format string, uses Local time and `yyyy-MM-dd HH:mm:ss.fff` (short form of ISO 8601)
        var local = value.Local.DateTime;

        // yyyy-
        var year = local.Year;
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
        var month = local.Month;
        if (month < 10)
        {
            AppendWithFillZero1(ref writer, month);
        }
        else
        {
            writer.AppendFormatted(month);
        }
        writer.AppendUtf8("-"u8);

        // 'dd '
        var day = local.Day;
        if (day < 10)
        {
            AppendWithFillZero1(ref writer, day);
        }
        else
        {
            writer.AppendFormatted(day);
        }
        writer.AppendUtf8(" "u8);

        // HH:
        var hour = local.Hour;
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
        var minute = local.Minute;
        if (minute < 10)
        {
            AppendWithFillZero1(ref writer, minute);
        }
        else
        {
            writer.AppendFormatted(minute);
        }
        writer.AppendUtf8(":"u8);

        // ss.
        var second = local.Second;
        if (second < 10)
        {
            AppendWithFillZero1(ref writer, second);
        }
        else
        {
            writer.AppendFormatted(second);
        }
        writer.AppendUtf8("."u8);

        // fff
        var millisecond = local.Millisecond; // The returned value is an integer between 0 and 999.
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
}
