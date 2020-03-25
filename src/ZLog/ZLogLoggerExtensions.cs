using Cysharp.Text;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ZLog.Entries;

namespace ZLog
{
    public static class ZLogLoggerExtensions
    {
        public static void ZDebug<T>(this ILogger logger, T payload)
        {
            logger.Log(LogLevel.Debug, default, new PayloadState<T>(payload), null, (state, ex) =>
            {
                // fallback for other logger.
               return JsonSerializer.Serialize(payload);
            });
        }

        public static void ZDebug<T1, T2>(this ILogger logger, string format, T1 arg1, T2 arg2)
        {
            logger.Log(LogLevel.Debug, default, new FormatLogState<T1, T2>(format, arg1, arg2), null, (state, ex) =>
            {
                // fallback for other logger.
                return ZString.Format(state.Format, state.Arg1, state.Arg2);
            });
        }
    }
}