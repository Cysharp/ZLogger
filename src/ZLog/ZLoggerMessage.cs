using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZLog
{
    // TODO:use ZLog.Prepare

    public static class ZLoggerMessage
    {
        public static Action<ILogger, Exception> Define(LogLevel logLevel, EventId eventId, string message)
        {
            return (ILogger logger, Exception ex) =>
            {
                logger.ZLogMessage(logLevel, eventId, ex, message);
            };
        }

        public static Action<ILogger, T0, Exception> Define<T0>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, Exception ex) =>
            {
                logger.ZLog<T0>(logLevel, eventId, ex, format, arg0);
            };
        }

        public static Action<ILogger, T0, T1, Exception> Define<T0, T1>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, Exception ex) =>
            {
                logger.ZLog<T0, T1>(logLevel, eventId, ex, format, arg0, arg1);
            };
        }

        public static Action<ILogger, T0, T1, T2, Exception> Define<T0, T1, T2>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2>(logLevel, eventId, ex, format, arg0, arg1, arg2);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, Exception> Define<T0, T1, T2, T3>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, Exception> Define<T0, T1, T2, T3, T4>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, Exception> Define<T0, T1, T2, T3, T4, T5>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, Exception> Define<T0, T1, T2, T3, T4, T5, T6>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            };
        }

        public static Action<ILogger, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Exception> Define<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Exception ex) =>
            {
                logger.ZLog<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(logLevel, eventId, ex, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            };
        }


        public static Action<ILogger, TPayload, T0, Exception> DefineWithPayload<TPayload, T0>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, Exception ex) =>
            {
                logger.ZLog<TPayload, T0>(logLevel, eventId, ex, payload, format, arg0);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, Exception> DefineWithPayload<TPayload, T0, T1>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1>(logLevel, eventId, ex, payload, format, arg0, arg1);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, Exception> DefineWithPayload<TPayload, T0, T1, T2>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            };
        }

        public static Action<ILogger, TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Exception> DefineWithPayload<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel logLevel, EventId eventId, string format)
        {
            return (ILogger logger, TPayload payload, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Exception ex) =>
            {
                logger.ZLog<TPayload, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(logLevel, eventId, ex, payload, format, arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            };
        }

    }
}
