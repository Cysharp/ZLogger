using System;
using ZLogger.MessagePack;

namespace ZLogger;

public static class ZLoggerOptionsMessagePackExtensions
{
    public static ZLoggerOptions UseMessagePackFormatter(this ZLoggerOptions options, Action<MessagePackZLoggerFormatter>? configure = null)
    {
        return options.UseFormatter(() =>
        {
            var formatter = new MessagePackZLoggerFormatter();
            configure?.Invoke(formatter);
            return formatter;
        });
    }
}