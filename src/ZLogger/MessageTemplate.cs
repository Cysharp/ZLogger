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
            if (chunk.Format == "short")
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
        // Timestamp, ignores alignment
        if (!value.TryFormat(ref writer, chunk.Format))
        {
            // uses Local DateTimeOffset
            writer.AppendFormatted(value.Local, chunk.Alignment, chunk.Format);
        }
    }
}
