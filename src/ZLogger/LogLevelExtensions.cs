using Microsoft.Extensions.Logging;
using System.Text;

namespace ZLogger;

public static class LogLevelExtensions
{
    public static ReadOnlySpan<byte> AsUtf8(this LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Trace:
                return "Trace"u8;
            case LogLevel.Debug:
                return "Debug"u8;
            case LogLevel.Information:
                return "Information"u8;
            case LogLevel.Warning:
                return "Warning"u8;
            case LogLevel.Error:
                return "Error"u8;
            case LogLevel.Critical:
                return "Critical"u8;
            case LogLevel.None:
                return "None"u8;
            default:
                return Encoding.UTF8.GetBytes(level.ToString());
        }
    }
}