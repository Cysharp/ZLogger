using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ZLogger.LogStates;

namespace ZLogger;

[InterpolatedStringHandler]
public ref struct ZLoggerTraceInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerTraceInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Trace, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerDebugInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerDebugInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Debug, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerInformationInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerInformationInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Information, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerWarningInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerWarningInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Warning, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerErrorInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerErrorInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Error, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerCriticalInterpolatedStringHandler
{
    public ZLoggerInterpolatedStringHandler InnerHandler;

    public ZLoggerCriticalInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        InnerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, logger, LogLevel.Critical, out enabled);
    }
    public void AppendLiteral([System.Diagnostics.CodeAnalysis.ConstantExpected]string s)
    {
        InnerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        InnerHandler.AppendFormatted(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(Nullable<T> value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
        where T : struct
    {
        InnerHandler.AppendFormatted<T>(value, alignment, format, argumentName);
    }

    public void AppendFormatted<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>((string, T) namedValue, int alignment = 0, string? format = null, string? _ = null)
    {
        InnerHandler.AppendFormatted(namedValue, alignment, format);
    }
}

