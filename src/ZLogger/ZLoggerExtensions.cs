using System;
using Microsoft.Extensions.Logging;

namespace ZLogger
{
    public static partial class ZLoggerExtensions
    {
        public static void ZLog(this ILogger logger, LogLevel logLevel, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, logLevel, default, null, in message);
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, logLevel, eventId, null, in message);
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            logger.Log(logLevel, default, message, exception, (state, ex) => state.ToString());
        }
        
        public static void ZLog(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            logger.Log(logLevel, eventId, message, exception, (state, ex) => state.ToString());
        }

        public static void ZLogTrace(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, default, null, in message);
        } 

        public static void ZLogTrace(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, eventId, null, in message);
        } 

        public static void ZLogTrace(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, default, exception, in message);
        } 

        public static void ZLogTrace(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Trace, eventId, exception, in message);
        }

        public static void ZLogDebug(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, default, null, in message);
        } 

        public static void ZLogDebug(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, eventId, null, in message);
        } 

        public static void ZLogDebug(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, default, exception, in message);
        } 

        public static void ZLogDebug(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Debug, eventId, exception, in message);
        }

        public static void ZLogInformation(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, default, null, in message);
        } 

        public static void ZLogInformation(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, eventId, null, in message);
        } 

        public static void ZLogInformation(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, default, exception, in message);
        } 

        public static void ZLogInformation(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Information, eventId, exception, in message);
        }

        public static void ZLogWarning(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, default, null, in message);
        } 

        public static void ZLogWarning(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, eventId, null, in message);
        } 

        public static void ZLogWarning(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, default, exception, in message);
        } 

        public static void ZLogWarning(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Warning, eventId, exception, in message);
        }

        public static void ZLogError(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, default, null, in message);
        } 

        public static void ZLogError(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, eventId, null, in message);
        } 

        public static void ZLogError(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, default, exception, in message);
        } 

        public static void ZLogError(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Error, eventId, exception, in message);
        }

        public static void ZLogCritical(this ILogger logger, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, default, null, in message);
        } 

        public static void ZLogCritical(this ILogger logger, EventId eventId, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, eventId, null, in message);
        } 

        public static void ZLogCritical(this ILogger logger, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, default, exception, in message);
        } 

        public static void ZLogCritical(this ILogger logger, EventId eventId, Exception? exception, in ZLoggerInterpolatedStringHandler message)
        {
            ZLog(logger, LogLevel.Critical, eventId, exception, in message);
        }
    }
}