using System.Collections.Concurrent;
using System.Text;

namespace ZLogger.Generator;

public enum MessageSegmentKind
{
    Text, IndexParameter, NameParameter
}

public class MessageSegment
{
    public required MessageSegmentKind Kind { get; init; }

    // Text
    public string TextSegment { get; init; } = default!;
    public string UnescapedTextSegment { get; init; } = default!;

    // Parameter
    public int IndexParameter { get; init; }
    public string NameParameter { get; init; } = default!;
    public string? AlternativeName { get; init; }
    public string? Alignment { get; init; }
    public string? FormatString { get; init; }

    public string GetPropertyName() => AlternativeName ?? NameParameter;

    public override string ToString()
    {
        if (Kind == MessageSegmentKind.Text) return TextSegment;

        var sb = new StringBuilder();
        sb.Append("{");

        if (Kind == MessageSegmentKind.NameParameter)
        {
            sb.Append(NameParameter);
        }
        else // IndexParameter
        {
            sb.Append(IndexParameter);
        }

        if (Alignment != null)
        {
            sb.Append(",");
            sb.Append(Alignment);
        }

        if (FormatString != null)
        {
            sb.Append(":");
            sb.Append(FormatString);
        }

        sb.Append("}");
        return sb.ToString();
    }
}

// {index|name[,alignment][:formatString]}
public static class MessageParser
{
    enum ParameterParseState
    {
        IndexOrName, Alignment, FormatString
    }

    public static bool TryParseFormat(string format, out MessageSegment[] segments)
    {
        if (format == null || format.Length == 0)
        {
            segments = Array.Empty<MessageSegment>();
            return true;
        }

        var list = new List<MessageSegment>();

        var span = format.AsSpan();
        var text = new StringBuilder();
        var unescaped = new StringBuilder();
        var parameter = new StringBuilder();
        var alignment = new StringBuilder();
        var formatString = new StringBuilder();
        var parameterState = ParameterParseState.IndexOrName;
        for (int i = 0; i < span.Length; i++)
        {
            // search `{`
            if (span[i] == '{')
            {
                if (i == span.Length - 1) // reach end
                {
                    // Syntax error, '}' expected.
                    segments = null!;
                    return false;
                }

                if (span[i + 1] == '{') // escaped
                {
                    text.Append("{{");
                    unescaped.Append('{');
                    i++;
                    continue;
                }
                else
                {
                    // add text segment 
                    if (text.Length != 0)
                    {
                        list.Add(new MessageSegment
                        {
                            Kind = MessageSegmentKind.Text,
                            TextSegment = text.ToString(),
                            UnescapedTextSegment = unescaped.ToString()
                        });
                        text.Clear();
                        unescaped.Clear();
                    }

                    // search parameter
                    for (int j = i + 1; j < span.Length; j++)
                    {
                        i = j;
                        if (parameterState == ParameterParseState.IndexOrName && span[j] == ',')
                        {
                            parameterState = ParameterParseState.Alignment;
                            continue;
                        }
                        else if ((parameterState == ParameterParseState.IndexOrName || parameterState == ParameterParseState.Alignment) && span[j] == ':')
                        {
                            parameterState = ParameterParseState.FormatString;
                            continue;
                        }
                        else if (span[j] == '}')
                        {
                            var p = parameter.ToString().Trim();
                            var isIndex = int.TryParse(p, out var index);

                            if (p.Length == 0)
                            {
                                // invalid parameter
                                segments = null!;
                                return false;
                            }

                            var formatStr = (formatString.Length != 0) ? formatString.ToString().Trim() : null;
                            if (TryParseCustomFormat(formatStr, out var altName, out var altFormat))
                            {
                                formatStr = altFormat;
                            }

                            list.Add(new MessageSegment
                            {
                                Kind = isIndex ? MessageSegmentKind.IndexParameter : MessageSegmentKind.NameParameter,
                                NameParameter = p,
                                IndexParameter = index,
                                Alignment = (alignment.Length != 0) ? alignment.ToString().Trim() : null,
                                AlternativeName = altName,
                                FormatString = formatStr,
                            });
                            parameter.Clear();
                            alignment.Clear();
                            formatString.Clear();
                            parameterState = ParameterParseState.IndexOrName;
                            break; // end search parameter
                        }
                        else if (j == span.Length - 1)
                        {
                            // Syntax error, '}' expected.
                            segments = null!;
                            return false;
                        }

                        switch (parameterState)
                        {
                            case ParameterParseState.IndexOrName:
                                parameter.Append(span[j]);
                                break;
                            case ParameterParseState.Alignment:
                                alignment.Append(span[j]);
                                break;
                            case ParameterParseState.FormatString:
                                formatString.Append(span[j]);
                                break;
                            default:
                                break;
                        }
                    }
                    continue;
                }
            }
            else if (span[i] == '}')
            {
                if (i == span.Length - 1 || span[i + 1] != '}')
                {
                    // Syntax error, must be escaped.
                    segments = null!;
                    return false;
                }

                text.Append("}}");
                unescaped.Append('}');
                i++;
                continue;
            }

            text.Append(span[i]);
            unescaped.Append(span[i]);
        }

        if (text.Length != 0)
        {
            list.Add(new MessageSegment
            {
                Kind = MessageSegmentKind.Text,
                TextSegment = text.ToString(),
                UnescapedTextSegment = unescaped.ToString()
            });
        }

        segments = list.ToArray();
        return true;
    }

    public static bool TryParseCustomFormat(string? format, out string? altName, out string? altFormat)
    {
        if (format == null || !format.StartsWith("@"))
        {
            altName = null;
            altFormat = null;
            return false;
        }

        var moreFormat = format.IndexOf(':');
        if (moreFormat == -1)
        {
            altName = format.AsSpan(1, format.Length - 1).ToString();
            altFormat = null;
        }
        else
        {
            altName = format.AsSpan(1, moreFormat - 1).ToString();
            altFormat = format.AsSpan(moreFormat + 1, format.Length - moreFormat - 1).ToString();
        }
        return true;
    }
}
