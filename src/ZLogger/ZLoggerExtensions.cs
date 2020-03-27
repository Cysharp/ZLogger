using System;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using ZLogger.Entries;

namespace ZLogger
{
    public static partial class ZLoggerExtensions
    {
        public static void ZLog<T0>(this ILogger logger, LogLevel logLevel, string format, T0 arg0)
        {
            ZLog<T0>(logger, logLevel, default, null, format, arg0);
        }

        public static void ZLog<T0>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, logLevel, eventId, null, format, arg0);
        }

        public static void ZLog<T0>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, logLevel, default, exception, format, arg0);
        }

        public static void ZLog<T0>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0>(null, format, arg0), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0);
            });
        }
        
        public static void ZLog<TPayload, T0>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, logLevel, default, null, payload, format, arg0);
        }

        public static void ZLog<TPayload, T0>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, logLevel, eventId, null, payload, format, arg0);
        }

        public static void ZLog<TPayload, T0>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, logLevel, default, exception, payload, format, arg0);
        }

        public static void ZLog<TPayload, T0>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0>(payload, format, arg0), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0);
            });
        }

        public static void ZLogTrace<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Trace, default, null, format, arg0);
        }

        public static void ZLogTrace<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Trace, eventId, null, format, arg0);
        }

        public static void ZLogTrace<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Trace, default, exception, format, arg0);
        }

        public static void ZLogTrace<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Trace, eventId, exception, format, arg0);
        }

        public static void ZLogTrace<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Trace, default, null, payload, format, arg0);
        }

        public static void ZLogTrace<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Trace, eventId, null, payload, format, arg0);
        }

        public static void ZLogTrace<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Trace, default, exception, payload, format, arg0);
        }

        public static void ZLogTrace<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0);
        }

        public static void ZLogDebug<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Debug, default, null, format, arg0);
        }

        public static void ZLogDebug<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Debug, eventId, null, format, arg0);
        }

        public static void ZLogDebug<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Debug, default, exception, format, arg0);
        }

        public static void ZLogDebug<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Debug, eventId, exception, format, arg0);
        }

        public static void ZLogDebug<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Debug, default, null, payload, format, arg0);
        }

        public static void ZLogDebug<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Debug, eventId, null, payload, format, arg0);
        }

        public static void ZLogDebug<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Debug, default, exception, payload, format, arg0);
        }

        public static void ZLogDebug<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0);
        }

        public static void ZLogInformation<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Information, default, null, format, arg0);
        }

        public static void ZLogInformation<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Information, eventId, null, format, arg0);
        }

        public static void ZLogInformation<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Information, default, exception, format, arg0);
        }

        public static void ZLogInformation<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Information, eventId, exception, format, arg0);
        }

        public static void ZLogInformation<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Information, default, null, payload, format, arg0);
        }

        public static void ZLogInformation<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Information, eventId, null, payload, format, arg0);
        }

        public static void ZLogInformation<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Information, default, exception, payload, format, arg0);
        }

        public static void ZLogInformation<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Information, eventId, exception, payload, format, arg0);
        }

        public static void ZLogWarning<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Warning, default, null, format, arg0);
        }

        public static void ZLogWarning<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Warning, eventId, null, format, arg0);
        }

        public static void ZLogWarning<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Warning, default, exception, format, arg0);
        }

        public static void ZLogWarning<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Warning, eventId, exception, format, arg0);
        }

        public static void ZLogWarning<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Warning, default, null, payload, format, arg0);
        }

        public static void ZLogWarning<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Warning, eventId, null, payload, format, arg0);
        }

        public static void ZLogWarning<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Warning, default, exception, payload, format, arg0);
        }

        public static void ZLogWarning<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0);
        }

        public static void ZLogError<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Error, default, null, format, arg0);
        }

        public static void ZLogError<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Error, eventId, null, format, arg0);
        }

        public static void ZLogError<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Error, default, exception, format, arg0);
        }

        public static void ZLogError<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Error, eventId, exception, format, arg0);
        }

        public static void ZLogError<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Error, default, null, payload, format, arg0);
        }

        public static void ZLogError<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Error, eventId, null, payload, format, arg0);
        }

        public static void ZLogError<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Error, default, exception, payload, format, arg0);
        }

        public static void ZLogError<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Error, eventId, exception, payload, format, arg0);
        }

        public static void ZLogCritical<T0>(this ILogger logger, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Critical, default, null, format, arg0);
        }

        public static void ZLogCritical<T0>(this ILogger logger, EventId eventId, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Critical, eventId, null, format, arg0);
        }

        public static void ZLogCritical<T0>(this ILogger logger, Exception exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Critical, default, exception, format, arg0);
        }

        public static void ZLogCritical<T0>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0)
        {
            ZLog<T0>(logger, LogLevel.Critical, eventId, exception, format, arg0);
        }

        public static void ZLogCritical<TPayload, T0>(this ILogger logger, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Critical, default, null, payload, format, arg0);
        }

        public static void ZLogCritical<TPayload, T0>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Critical, eventId, null, payload, format, arg0);
        }

        public static void ZLogCritical<TPayload, T0>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Critical, default, exception, payload, format, arg0);
        }

        public static void ZLogCritical<TPayload, T0>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0)
        {
            ZLog<TPayload, T0>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0);
        }


        public static void ZLog<T0, T1>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, logLevel, default, null, format, arg0, arg1);
        }

        public static void ZLog<T0, T1>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, logLevel, eventId, null, format, arg0, arg1);
        }

        public static void ZLog<T0, T1>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, logLevel, default, exception, format, arg0, arg1);
        }

        public static void ZLog<T0, T1>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1>(null, format, arg0, arg1), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1);
            });
        }
        
        public static void ZLog<TPayload, T0, T1>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, logLevel, default, null, payload, format, arg0, arg1);
        }

        public static void ZLog<TPayload, T0, T1>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, logLevel, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLog<TPayload, T0, T1>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, logLevel, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLog<TPayload, T0, T1>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1>(payload, format, arg0, arg1), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1);
            });
        }

        public static void ZLogTrace<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Trace, default, null, format, arg0, arg1);
        }

        public static void ZLogTrace<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1);
        }

        public static void ZLogTrace<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Trace, default, exception, format, arg0, arg1);
        }

        public static void ZLogTrace<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogTrace<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogTrace<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogTrace<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogTrace<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1);
        }

        public static void ZLogDebug<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Debug, default, null, format, arg0, arg1);
        }

        public static void ZLogDebug<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1);
        }

        public static void ZLogDebug<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Debug, default, exception, format, arg0, arg1);
        }

        public static void ZLogDebug<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogDebug<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogDebug<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogDebug<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogDebug<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1);
        }

        public static void ZLogInformation<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Information, default, null, format, arg0, arg1);
        }

        public static void ZLogInformation<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Information, eventId, null, format, arg0, arg1);
        }

        public static void ZLogInformation<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Information, default, exception, format, arg0, arg1);
        }

        public static void ZLogInformation<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogInformation<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogInformation<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogInformation<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogInformation<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1);
        }

        public static void ZLogWarning<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Warning, default, null, format, arg0, arg1);
        }

        public static void ZLogWarning<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1);
        }

        public static void ZLogWarning<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Warning, default, exception, format, arg0, arg1);
        }

        public static void ZLogWarning<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogWarning<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogWarning<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogWarning<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogWarning<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1);
        }

        public static void ZLogError<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Error, default, null, format, arg0, arg1);
        }

        public static void ZLogError<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Error, eventId, null, format, arg0, arg1);
        }

        public static void ZLogError<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Error, default, exception, format, arg0, arg1);
        }

        public static void ZLogError<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogError<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogError<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogError<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogError<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1);
        }

        public static void ZLogCritical<T0, T1>(this ILogger logger, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Critical, default, null, format, arg0, arg1);
        }

        public static void ZLogCritical<T0, T1>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1);
        }

        public static void ZLogCritical<T0, T1>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Critical, default, exception, format, arg0, arg1);
        }

        public static void ZLogCritical<T0, T1>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1)
        {
            ZLog<T0, T1>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1);
        }

        public static void ZLogCritical<TPayload, T0, T1>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1);
        }

        public static void ZLogCritical<TPayload, T0, T1>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1);
        }

        public static void ZLogCritical<TPayload, T0, T1>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1);
        }

        public static void ZLogCritical<TPayload, T0, T1>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1)
        {
            ZLog<TPayload, T0, T1>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1);
        }


        public static void ZLog<T0, T1, T2>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, logLevel, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLog<T0, T1, T2>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, logLevel, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLog<T0, T1, T2>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, logLevel, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLog<T0, T1, T2>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2>(null, format, arg0, arg1, arg2), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLog<TPayload, T0, T1, T2>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLog<TPayload, T0, T1, T2>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLog<TPayload, T0, T1, T2>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2>(payload, format, arg0, arg1, arg2), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2);
            });
        }

        public static void ZLogTrace<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogError<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogError<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogError<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogError<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogError<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogError<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogError<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogError<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<T0, T1, T2>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<T0, T1, T2>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<T0, T1, T2>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<T0, T1, T2>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2)
        {
            ZLog<TPayload, T0, T1, T2>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2);
        }


        public static void ZLog<T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3>(null, format, arg0, arg1, arg2, arg3), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3>(payload, format, arg0, arg1, arg2, arg3), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<T0, T1, T2, T3>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<T0, T1, T2, T3>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<T0, T1, T2, T3>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<T0, T1, T2, T3>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            ZLog<TPayload, T0, T1, T2, T3>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3);
        }


        public static void ZLog<T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4>(null, format, arg0, arg1, arg2, arg3, arg4), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4>(payload, format, arg0, arg1, arg2, arg3, arg4), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<T0, T1, T2, T3, T4>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5>(null, format, arg0, arg1, arg2, arg3, arg4, arg5), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }


        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14, state.Arg15);
            });
        }
        
        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, logLevel, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15), exception, (state, ex) =>
            {
                return ZString.Format(state.Format, state.Arg0, state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14, state.Arg15);
            });
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogTrace<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Trace, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogDebug<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Debug, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogInformation<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Information, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogWarning<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Warning, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogError<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Error, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, default, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, eventId, null, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, default, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, eventId, exception, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, default, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, eventId, null, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, Exception exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, default, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        public static void ZLogCritical<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string format, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(logger, LogLevel.Critical, eventId, exception, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }


        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, string message)
        {
            ZLogMessage(logger, logLevel, default, null, message);
        }

        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, EventId eventId, string message)
        {
            ZLogMessage(logger, logLevel, eventId, null, message);
        }

        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, Exception exception, string message)
        {
            ZLogMessage<object>(logger, logLevel, default, exception, null!, message);
        }

        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string message)
        {
            logger.Log(logLevel, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, TPayload payload, string message)
        {
            ZLogMessage<TPayload>(logger, logLevel, default, null, payload, message);
        }

        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string message)
        {
            ZLogMessage<TPayload>(logger, logLevel, eventId, null, payload, message);
        }

        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string message)
        {
            ZLogMessage<TPayload>(logger, logLevel, default, exception, payload, message);
        }

        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(logLevel, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogTraceMessage(this ILogger logger, string message)
        {
            ZLogTraceMessage(logger, default, null, message);
        }

        public static void ZLogTraceMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogTraceMessage(logger, eventId, null, message);
        }

        public static void ZLogTraceMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogTraceMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogTraceMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Trace, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogTraceMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogTraceMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogTraceMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogTraceMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogTraceMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogTraceMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogTraceMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Trace, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogDebugMessage(this ILogger logger, string message)
        {
            ZLogDebugMessage(logger, default, null, message);
        }

        public static void ZLogDebugMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogDebugMessage(logger, eventId, null, message);
        }

        public static void ZLogDebugMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogDebugMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogDebugMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Debug, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogDebugMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogDebugMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogDebugMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogDebugMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogDebugMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogDebugMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogDebugMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Debug, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogInformationMessage(this ILogger logger, string message)
        {
            ZLogInformationMessage(logger, default, null, message);
        }

        public static void ZLogInformationMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogInformationMessage(logger, eventId, null, message);
        }

        public static void ZLogInformationMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogInformationMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogInformationMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Information, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogInformationMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogInformationMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogInformationMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogInformationMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogInformationMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogInformationMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogInformationMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Information, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogWarningMessage(this ILogger logger, string message)
        {
            ZLogWarningMessage(logger, default, null, message);
        }

        public static void ZLogWarningMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogWarningMessage(logger, eventId, null, message);
        }

        public static void ZLogWarningMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogWarningMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogWarningMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Warning, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogWarningMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogWarningMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogWarningMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogWarningMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogWarningMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogWarningMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogWarningMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Warning, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogErrorMessage(this ILogger logger, string message)
        {
            ZLogErrorMessage(logger, default, null, message);
        }

        public static void ZLogErrorMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogErrorMessage(logger, eventId, null, message);
        }

        public static void ZLogErrorMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogErrorMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogErrorMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Error, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogErrorMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogErrorMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogErrorMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogErrorMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogErrorMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogErrorMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogErrorMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Error, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }

        public static void ZLogCriticalMessage(this ILogger logger, string message)
        {
            ZLogCriticalMessage(logger, default, null, message);
        }

        public static void ZLogCriticalMessage(this ILogger logger, EventId eventId, string message)
        {
            ZLogCriticalMessage(logger, eventId, null, message);
        }

        public static void ZLogCriticalMessage(this ILogger logger, Exception exception, string message)
        {
            ZLogCriticalMessage<object>(logger, default, exception, null!, message);
        }

        public static void ZLogCriticalMessage(this ILogger logger, EventId eventId, Exception? exception, string message)
        {
            logger.Log(LogLevel.Critical, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
            {
                return state.Format;
            });
        }

        public static void ZLogCriticalMessage<TPayload>(this ILogger logger, TPayload payload, string message)
        {
            ZLogCriticalMessage<TPayload>(logger, default, null, payload, message);
        }

        public static void ZLogCriticalMessage<TPayload>(this ILogger logger, EventId eventId, TPayload payload, string message)
        {
            ZLogCriticalMessage<TPayload>(logger, eventId, null, payload, message);
        }

        public static void ZLogCriticalMessage<TPayload>(this ILogger logger, Exception exception, TPayload payload, string message)
        {
            ZLogCriticalMessage<TPayload>(logger, default, exception, payload, message);
        }

        public static void ZLogCriticalMessage<TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload, string message)
        {
            logger.Log(LogLevel.Critical, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
            {
                return message;
            });
        }


    }
}