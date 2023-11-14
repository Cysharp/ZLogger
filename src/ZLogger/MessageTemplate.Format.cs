using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Utf8StringInterpolation;

namespace ZLogger;

public readonly partial struct MessageTemplate
{
	public void Format<T0>(T0 arg0)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1>(T0 arg0, T1 arg1)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2>(T0 arg0, T1 arg1, T2 arg2)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3>(T0 arg0, T1 arg1, T2 arg2, T3 arg3)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    case 11:
                        AppendFormatted(ref stringWriter, ref arg11, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    case 11:
                        AppendFormatted(ref stringWriter, ref arg11, ref item);
                        break;
                    case 12:
                        AppendFormatted(ref stringWriter, ref arg12, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    case 11:
                        AppendFormatted(ref stringWriter, ref arg11, ref item);
                        break;
                    case 12:
                        AppendFormatted(ref stringWriter, ref arg12, ref item);
                        break;
                    case 13:
                        AppendFormatted(ref stringWriter, ref arg13, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    case 11:
                        AppendFormatted(ref stringWriter, ref arg11, ref item);
                        break;
                    case 12:
                        AppendFormatted(ref stringWriter, ref arg12, ref item);
                        break;
                    case 13:
                        AppendFormatted(ref stringWriter, ref arg13, ref item);
                        break;
                    case 14:
                        AppendFormatted(ref stringWriter, ref arg14, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}

	public void Format<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
	{
	    var stringWriter = new Utf8StringWriter<IBufferWriter<byte>>(template.LiteralLength, template.FormattedCount, writer);

        foreach (ref var item in template.TemplateChunk.AsSpan())
        {
            if (item.Index == -1)
            {
                stringWriter.AppendUtf8(item.Literal);
            }
            else
            {
                switch (item.Index)
                {
                    case 0:
                        AppendFormatted(ref stringWriter, ref arg0, ref item);
                        break;
                    case 1:
                        AppendFormatted(ref stringWriter, ref arg1, ref item);
                        break;
                    case 2:
                        AppendFormatted(ref stringWriter, ref arg2, ref item);
                        break;
                    case 3:
                        AppendFormatted(ref stringWriter, ref arg3, ref item);
                        break;
                    case 4:
                        AppendFormatted(ref stringWriter, ref arg4, ref item);
                        break;
                    case 5:
                        AppendFormatted(ref stringWriter, ref arg5, ref item);
                        break;
                    case 6:
                        AppendFormatted(ref stringWriter, ref arg6, ref item);
                        break;
                    case 7:
                        AppendFormatted(ref stringWriter, ref arg7, ref item);
                        break;
                    case 8:
                        AppendFormatted(ref stringWriter, ref arg8, ref item);
                        break;
                    case 9:
                        AppendFormatted(ref stringWriter, ref arg9, ref item);
                        break;
                    case 10:
                        AppendFormatted(ref stringWriter, ref arg10, ref item);
                        break;
                    case 11:
                        AppendFormatted(ref stringWriter, ref arg11, ref item);
                        break;
                    case 12:
                        AppendFormatted(ref stringWriter, ref arg12, ref item);
                        break;
                    case 13:
                        AppendFormatted(ref stringWriter, ref arg13, ref item);
                        break;
                    case 14:
                        AppendFormatted(ref stringWriter, ref arg14, ref item);
                        break;
                    case 15:
                        AppendFormatted(ref stringWriter, ref arg15, ref item);
                        break;
                    default:
                        throw new FormatException("Index (zero based) must be greater than or equal to zero and less than the size of the argument list.");
                }
            }
        }

        stringWriter.Flush();
	}


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static void AppendFormatted<T>(ref Utf8StringWriter<IBufferWriter<byte>> writer, ref T value, ref MessageTemplateChunk chunk)
    {
        // JIT eliminates typeof(T) == typeof(...).
        if (typeof(T) == typeof(LogLevel))
        {
            AppendLogLevel(ref writer, ref Unsafe.As<T, LogLevel>(ref value), ref chunk);
        }
        else if (typeof(T) == typeof(LogCategory))
        {
            AppendLogCategory(ref writer, ref Unsafe.As<T, LogCategory>(ref value), ref chunk);
        }
        else if (typeof(T) == typeof(Timestamp))
        {
            AppendTimestamp(ref writer, ref Unsafe.As<T, Timestamp>(ref value), ref chunk);
        }
        else if (typeof(T) == typeof(string))
        {
            writer.AppendFormatted(Unsafe.As<T, string>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(char))
        {
            writer.AppendFormatted(Unsafe.As<T, char>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(char?))
        {
            writer.AppendFormatted(Unsafe.As<T, char?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(bool))
        {
            writer.AppendFormatted(Unsafe.As<T, bool>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(bool?))
        {
            writer.AppendFormatted(Unsafe.As<T, bool?>(ref value), chunk.Alignment, chunk.Format);
        }
#if !NET8_0_OR_GREATER
        else if (typeof(T) == typeof(byte))
        {
            writer.AppendFormatted(Unsafe.As<T, byte>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(byte?))
        {
            writer.AppendFormatted(Unsafe.As<T, byte?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Decimal))
        {
            writer.AppendFormatted(Unsafe.As<T, Decimal>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Decimal?))
        {
            writer.AppendFormatted(Unsafe.As<T, Decimal?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Double))
        {
            writer.AppendFormatted(Unsafe.As<T, Double>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Double?))
        {
            writer.AppendFormatted(Unsafe.As<T, Double?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Guid))
        {
            writer.AppendFormatted(Unsafe.As<T, Guid>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Guid?))
        {
            writer.AppendFormatted(Unsafe.As<T, Guid?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int16))
        {
            writer.AppendFormatted(Unsafe.As<T, Int16>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int16?))
        {
            writer.AppendFormatted(Unsafe.As<T, Int16?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int32))
        {
            writer.AppendFormatted(Unsafe.As<T, Int32>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int32?))
        {
            writer.AppendFormatted(Unsafe.As<T, Int32?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int64))
        {
            writer.AppendFormatted(Unsafe.As<T, Int64>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Int64?))
        {
            writer.AppendFormatted(Unsafe.As<T, Int64?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(SByte))
        {
            writer.AppendFormatted(Unsafe.As<T, SByte>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(SByte?))
        {
            writer.AppendFormatted(Unsafe.As<T, SByte?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Single))
        {
            writer.AppendFormatted(Unsafe.As<T, Single>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(Single?))
        {
            writer.AppendFormatted(Unsafe.As<T, Single?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt16))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt16>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt16?))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt16?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt32))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt32>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt32?))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt32?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt64))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt64>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(UInt64?))
        {
            writer.AppendFormatted(Unsafe.As<T, UInt64?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(DateTime))
        {
            writer.AppendFormatted(Unsafe.As<T, DateTime>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(DateTime?))
        {
            writer.AppendFormatted(Unsafe.As<T, DateTime?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(DateTimeOffset))
        {
            writer.AppendFormatted(Unsafe.As<T, DateTimeOffset>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(DateTimeOffset?))
        {
            writer.AppendFormatted(Unsafe.As<T, DateTimeOffset?>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(TimeSpan))
        {
            writer.AppendFormatted(Unsafe.As<T, TimeSpan>(ref value), chunk.Alignment, chunk.Format);
        }
        else if (typeof(T) == typeof(TimeSpan?))
        {
            writer.AppendFormatted(Unsafe.As<T, TimeSpan?>(ref value), chunk.Alignment, chunk.Format);
        }
#endif
        else
        {
            writer.AppendFormatted(value, chunk.Alignment, chunk.Format);
        }
    }
}