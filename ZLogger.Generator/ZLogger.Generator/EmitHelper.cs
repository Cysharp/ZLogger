using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace ZLogger.Generator;
internal static class EmitHelper
{
    public static string ToCode(this Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.NotApplicable:
                return "";
            case Accessibility.Private:
                return "private";
            case Accessibility.ProtectedAndInternal:
                return "private protected";
            case Accessibility.Protected:
                return "protected";
            case Accessibility.Internal:
                return "internal";
            case Accessibility.ProtectedOrInternal:
                return "protected internal";
            case Accessibility.Public:
                return "public";
            default:
                return "";
        }
    }

    public static string ToParameterPrefix(this RefKind kind)
    {
        switch (kind)
        {
            case RefKind.Out: return "out ";
            case RefKind.Ref: return "ref ";
            case RefKind.In: return "in ";
            // case RefKind.RefReadOnlyParameter: return "ref readonly ";
            case (RefKind)4: return "ref readonly ";
            case RefKind.None: return "";
            default: return "";
        }
    }

    public static string ToUseParameterPrefix(this RefKind kind)
    {
        switch (kind)
        {
            case RefKind.Out: return "out ";
            case RefKind.Ref: return "ref ";
            case RefKind.In: return "in ";
            case (RefKind)4: return "in "; // ref readonly
            case RefKind.None: return "";
            default: return "";
        }
    }

    public static string ToFullyQualifiedFormatString(this ISymbol symbol)
    {
        return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public static bool EqualsUnconstructedGenericType(this INamedTypeSymbol left, INamedTypeSymbol right)
    {
        var l = left.IsGenericType ? left.ConstructUnboundGenericType() : left;
        var r = right.IsGenericType ? right.ConstructUnboundGenericType() : right;
        return SymbolEqualityComparer.Default.Equals(l, r);
    }

    public static string StringJoin(this IEnumerable<string> values, string separator)
    {
        return string.Join(separator, values);
    }

    public static string StringJoinNewLine(this IEnumerable<string> values)
    {
        return string.Join(Environment.NewLine, values);
    }

    public static string StringJoinComma(this IEnumerable<string> values)
    {
        return string.Join(", ", values);
    }

    public static string ForEachLine<T>(string indent, IEnumerable<T> values, Func<T, string> lineSelector)
    {
        return string.Join(Environment.NewLine, values.Select(x => indent + lineSelector(x)));
    }

    public static string ForEachLine<T>(string indent, IEnumerable<T> values, Func<T, int, string> lineSelector)
    {
        return string.Join(Environment.NewLine, values.Select((x, i) => indent + lineSelector(x, i)));
    }

    public static string ForLine(string indent, int begin, int end, Func<int, string> lineSelector)
    {
        return string.Join(Environment.NewLine, Enumerable.Range(begin, end - begin).Select(x => indent + lineSelector(x)));
    }

    public static string If(bool condition, string code)
    {
        return condition ? code : "";
    }

    public static string If(bool condition, string ifCode, string elseCode)
    {
        return condition ? ifCode : elseCode;
    }
}