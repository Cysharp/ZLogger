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
}
