using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace ZLogger
{
    public static partial class ZLoggerExtensions
    {
        public static void ZLog(this ILogger logger, LogLevel logLevel, [InterpolatedStringHandlerArgument("logger", "logLevel")]ref ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, logLevel, default, null, ref message);
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, EventId eventId, [InterpolatedStringHandlerArgument("logger", "logLevel")]ref ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, logLevel, eventId, null, ref message);
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, Exception? exception, [InterpolatedStringHandlerArgument("logger", "logLevel")]ref ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, logLevel, default, exception, ref message);
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, [InterpolatedStringHandlerArgument("logger", "logLevel")]ref ZLoggerInterpolatedStringHandler message)
        {
            if (message.IsLoggerEnabled)
            {
                using var state = message.GetStateAndClear();
                logger.Log(logLevel, eventId, state, exception, static (state, ex) => state.ToString());
            }
        }

        public static void ZLogTrace(this ILogger logger, ref ZLoggerTraceInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, default, null, ref message.innerHandler);
        } 

        public static void ZLogTrace(this ILogger logger, EventId eventId, ref ZLoggerTraceInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogTrace(this ILogger logger, Exception? exception, ref ZLoggerTraceInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, default, exception, ref message.innerHandler);
        } 

        public static void ZLogTrace(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerTraceInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, eventId, exception, ref message.innerHandler);
        }

        public static void ZLogDebug(this ILogger logger, ref ZLoggerDebugInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, default, null, ref message.innerHandler);
        } 

        public static void ZLogDebug(this ILogger logger, EventId eventId, ref ZLoggerDebugInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogDebug(this ILogger logger, Exception? exception, ref ZLoggerDebugInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, default, exception, ref message.innerHandler);
        } 

        public static void ZLogDebug(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerDebugInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, eventId, exception, ref message.innerHandler);
        }

        public static void ZLogInformation(this ILogger logger, ref ZLoggerInformationInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, default, null, ref message.innerHandler);
        } 

        public static void ZLogInformation(this ILogger logger, EventId eventId, ref ZLoggerInformationInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogInformation(this ILogger logger, Exception? exception, ref ZLoggerInformationInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, default, exception, ref message.innerHandler);
        } 

        public static void ZLogInformation(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerInformationInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, eventId, exception, ref message.innerHandler);
        }

        public static void ZLogWarning(this ILogger logger, ref ZLoggerWarningInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, default, null, ref message.innerHandler);
        } 

        public static void ZLogWarning(this ILogger logger, EventId eventId, ref ZLoggerWarningInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogWarning(this ILogger logger, Exception? exception, ref ZLoggerWarningInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, default, exception, ref message.innerHandler);
        } 

        public static void ZLogWarning(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerWarningInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, eventId, exception, ref message.innerHandler);
        }

        public static void ZLogError(this ILogger logger, ref ZLoggerErrorInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, default, null, ref message.innerHandler);
        } 

        public static void ZLogError(this ILogger logger, EventId eventId, ref ZLoggerErrorInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogError(this ILogger logger, Exception? exception, ref ZLoggerErrorInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, default, exception, ref message.innerHandler);
        } 

        public static void ZLogError(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerErrorInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, eventId, exception, ref message.innerHandler);
        }

        public static void ZLogCritical(this ILogger logger, ref ZLoggerCriticalInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, default, null, ref message.innerHandler);
        } 

        public static void ZLogCritical(this ILogger logger, EventId eventId, ref ZLoggerCriticalInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, eventId, null, ref message.innerHandler);
        } 

        public static void ZLogCritical(this ILogger logger, Exception? exception, ref ZLoggerCriticalInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, default, exception, ref message.innerHandler);
        } 

        public static void ZLogCritical(this ILogger logger, EventId eventId, Exception? exception, ref ZLoggerCriticalInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, eventId, exception, ref message.innerHandler);
        }
    }
}
