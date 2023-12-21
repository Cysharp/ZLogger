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
        [InterpolatedStringHandlerArgument("logger", "logLevel")] ref ZLoggerInterpolatedStringHandler message, 
        UnityEngine.Object context)
    {
        if (message.IsLoggerEnabled)
        {
            var state = message.GetState();
			var unityState = new UnityDebugLogState(state, context);
            try
            {
                logger.Log(logLevel, default, unityState, null, static (state, ex) => state.ToString());
            }
            finally
            {
                state.Release();
            }
        }
    }
}

