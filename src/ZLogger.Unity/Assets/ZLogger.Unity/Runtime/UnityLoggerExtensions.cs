using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using ZLogger.LogStates;

namespace ZLogger.Unity;

public static class UnityLoggerExtensions
{
	[UnityEngine.HideInCallstack]
    public static void ZLog(
        this ILogger logger, 
        LogLevel logLevel,
        Exception? exception,
        [InterpolatedStringHandlerArgument("logger", "logLevel")] ref ZLoggerInterpolatedStringHandler message, 
        UnityEngine.Object context)
    {
        if (message.IsLoggerEnabled)
        {
            var state = message.GetState();
			var unityState = new UnityDebugLogState(state, context);
            try
            {
                logger.Log(logLevel, default, unityState, exception, static (state, ex) => state.ToString());
            }
            finally
            {
                state.Release();
            }
        }
    }
    
    [UnityEngine.HideInCallstack]
    public static void ZLog(this ILogger logger, LogLevel logLevel, [InterpolatedStringHandlerArgument("logger", "logLevel")] ref ZLoggerInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, logLevel, null, ref message, context);
    }

    [UnityEngine.HideInCallstack]
    public static void ZLogTrace(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerTraceInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Trace, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogTrace(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerTraceInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Trace, exception, ref message.InnerHandler, context);
    }
    [UnityEngine.HideInCallstack]
    public static void ZLogDebug(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerDebugInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Debug, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogDebug(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerDebugInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Debug, exception, ref message.InnerHandler, context);
    }
    [UnityEngine.HideInCallstack]
    public static void ZLogInformation(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerInformationInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Information, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogInformation(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerInformationInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Information, exception, ref message.InnerHandler, context);
    }
    [UnityEngine.HideInCallstack]
    public static void ZLogWarning(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerWarningInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Warning, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogWarning(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerWarningInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Warning, exception, ref message.InnerHandler, context);
    }
    [UnityEngine.HideInCallstack]
    public static void ZLogError(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerErrorInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Error, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogError(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerErrorInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Error, exception, ref message.InnerHandler, context);
    }
    [UnityEngine.HideInCallstack]
    public static void ZLogCritical(this ILogger logger, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerCriticalInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Critical, ref message.InnerHandler, context);
    }
 
    [UnityEngine.HideInCallstack]
    public static void ZLogCritical(this ILogger logger, Exception? exception, [InterpolatedStringHandlerArgument("logger")] ref ZLoggerCriticalInterpolatedStringHandler message, UnityEngine.Object context)
    {
        ZLog(logger, LogLevel.Critical, exception, ref message.InnerHandler, context);
    }
}

