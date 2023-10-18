using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ZLogger.LogStates;

namespace ZLogger;

[InterpolatedStringHandler]
public ref struct ZLoggerTraceInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerTraceInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Trace, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerDebugInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerDebugInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Debug, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerInformationInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerInformationInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Information, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerWarningInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerWarningInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Warning, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerErrorInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerErrorInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Error, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

[InterpolatedStringHandler]
public ref struct ZLoggerCriticalInterpolatedStringHandler
{
    internal ZLoggerInterpolatedStringHandler innerHandler;

    public ZLoggerCriticalInterpolatedStringHandler(int literalLength, int formattedCount, ILogger logger, out bool enabled)
    {
        this.innerHandler = new ZLoggerInterpolatedStringHandler(literalLength, formattedCount, LogLevel.Critical, logger, out enabled);
    }
    public void AppendLiteral(string s)
    {
        this.innerHandler.AppendLiteral(s);
    }

    public void AppendFormatted<T>(T value, int alignment = 0, string? format = null, [CallerArgumentExpression("value")] string? argumentName = null)
    {
        this.innerHandler.AppendFormatted(value, alignment, format, argumentName);
    }
}

