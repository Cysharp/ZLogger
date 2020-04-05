using Cysharp.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using ZLogger.Entries;

namespace ZLogger
{
    public static class ZLoggerMessage
    {
        public static Action<ILogger, Exception> Define(LogLevel logLevel, EventId eventId, string message)
        {
            return (ILogger logger, Exception ex) =>
            {
                logger.ZLog(logLevel, eventId, ex, message);
            };
        }

        public static Action<ILogger, T1, Exception> Define<T1>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1>(format);

            return (ILogger logger, T1 arg1, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1>(null, prepared, arg1), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1);
                });
            };
        }

        public static Action<ILogger, T1, T2, Exception> Define<T1, T2>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2>(format);

            return (ILogger logger, T1 arg1, T2 arg2, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2>(null, prepared, arg1, arg2), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, Exception> Define<T1, T2, T3>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3>(null, prepared, arg1, arg2, arg3), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, Exception> Define<T1, T2, T3, T4>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4>(null, prepared, arg1, arg2, arg3, arg4), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, Exception> Define<T1, T2, T3, T4, T5>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5>(null, prepared, arg1, arg2, arg3, arg4, arg5), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, Exception> Define<T1, T2, T3, T4, T5, T6>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, Exception> Define<T1, T2, T3, T4, T5, T6, T7>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                });
            };
        }

        public static Action<ILogger, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Exception> Define<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(format);

            return (ILogger logger, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(null, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13, state.Arg14);
                });
            };
        }


        public static Action<ILogger, TPayload, T1, Exception> DefineWithPayload<TPayload, T1>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1>(format);

            return (ILogger logger, TPayload payload, T1 arg1, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1>(payload, prepared, arg1), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, Exception> DefineWithPayload<TPayload, T1, T2>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2>(payload, prepared, arg1, arg2), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, Exception> DefineWithPayload<TPayload, T1, T2, T3>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3>(payload, prepared, arg1, arg2, arg3), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4>(payload, prepared, arg1, arg2, arg3, arg4), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5>(payload, prepared, arg1, arg2, arg3, arg4, arg5), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12);
                });
            };
        }

        public static Action<ILogger, TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Exception> DefineWithPayload<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(LogLevel logLevel, EventId eventId, string format)
        {
            var prepared = ZString.PrepareUtf8<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(format);

            return (ILogger logger, TPayload payload, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Exception ex) =>
            {
                logger.Log(logLevel, eventId, new PreparedFormatLogState<TPayload, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(payload, prepared, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13), ex, (state, _) =>
                {
                    return state.Format.Format(state.Arg1, state.Arg2, state.Arg3, state.Arg4, state.Arg5, state.Arg6, state.Arg7, state.Arg8, state.Arg9, state.Arg10, state.Arg11, state.Arg12, state.Arg13);
                });
            };
        }

    }
}
