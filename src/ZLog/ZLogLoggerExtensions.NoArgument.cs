//using System;
//using Cysharp.Text;
//using Microsoft.Extensions.Logging;
//using ZLog.Entries;

//namespace ZLog
//{
//    public static partial class ZLogLoggerExtensions
//    {
//        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, string message)
//        {
//            logger.ZLogMessage(LogLevel.Debug, "takoyaki", "foo");

//            ZLogMessage(logger, logLevel, default, null, message);
//        }

//        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, EventId eventId, string message)
//        {
//            ZLogMessage(logger, logLevel, eventId, null, message);
//        }

//        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, Exception exception, string message)
//        {
//            ZLogMessage<object>(logger, logLevel, default, exception, null!, message);
//        }

//        public static void ZLogMessage(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string message)
//        {
//            logger.Log(logLevel, eventId, new FormatLogState<object, object>(null, message, null!), exception, (state, ex) =>
//            {
//                return state.Format;
//            });
//        }

//        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, TPayload payload, string message)
//        {
//            ZLogMessage<TPayload>(logger, logLevel, default, null, payload, message);
//        }

//        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, EventId eventId, TPayload payload, string message)
//        {
//            ZLogMessage<TPayload>(logger, logLevel, eventId, null, payload, message);
//        }

//        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, Exception exception, TPayload payload, string message)
//        {
//            ZLogMessage<TPayload>(logger, logLevel, default, exception, payload, message);
//        }

//        public static void ZLogMessage<TPayload>(this ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, TPayload payload, string message)
//        {
//            logger.Log(logLevel, eventId, new FormatLogState<TPayload, object>(payload, message, null!), exception, (state, ex) =>
//            {
//                return message;
//            });
//        }

//        //        public static void ZLog<#= logLevel #>(this ILogger logger)
//        //        {
//        //    ZLog(logger, LogLevel.<#= logLevel #>, default, null, format);
//        //        }

//        //public static void ZLog<#= logLevel #>(this ILogger logger, EventId eventId)
//        //        {
//        //    ZLog(logger, LogLevel.<#= logLevel #>, eventId, null, format);
//        //        }

//        //public static void ZLog<#= logLevel #>(this ILogger logger, Exception exception)
//        //        {
//        //    ZLog(logger, LogLevel.<#= logLevel #>, default, exception, format);
//        //        }

//        //public static void ZLog<#= logLevel #>(this ILogger logger, EventId eventId, Exception? exception)
//        //        {
//        //    ZLog(logger, LogLevel.<#= logLevel #>, eventId, exception, format);
//        //        }

//        //public static void ZLog<#= logLevel #><TPayload>(this ILogger logger, TPayload payload)
//        //        {
//        //    ZLog<TPayload>(logger, LogLevel.<#= logLevel #>, default, null, payload, format);
//        //        }

//        //public static void ZLog<#= logLevel #><TPayload>(this ILogger logger, EventId eventId, TPayload payload)
//        //        {
//        //    ZLog<TPayload>(logger, LogLevel.<#= logLevel #>, eventId, null, payload, format);
//        //        }

//        //public static void ZLog<#= logLevel #><TPayload>(this ILogger logger, Exception exception, TPayload payload)
//        //        {
//        //    ZLog<TPayload>(logger, LogLevel.<#= logLevel #>, default, exception, payload, format);
//        //        }

//        //public static void ZLog<#= logLevel #><TPayload>(this ILogger logger, EventId eventId, Exception? exception, TPayload payload)
//        //        {
//        //    ZLog<TPayload>(logger, LogLevel.<#= logLevel #>, eventId, exception, payload, format);
//        //        }

//    }
//}