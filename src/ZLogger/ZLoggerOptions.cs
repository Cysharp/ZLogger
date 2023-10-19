using ZLogger.Formatters;
using Microsoft.Extensions.Logging;
using ZLogger.Formatters;

namespace ZLogger
{
    public class ZLoggerOptions
    {
        public Action<LogInfo, Exception>? InternalErrorLogger { get; set; }
        
        public TimeSpan? FlushRate { get; set; }
        public bool IncludeScopes { get; set; }
        public IKeyNameMutator? KeyNameMutator { get; set; }
        public LogLevel LogToErrorThreshold { get; set; } = LogLevel.None;

        Func<IZLoggerFormatter> formatterFactory = DefaultFormatterFactory;

        public IZLoggerFormatter CreateFormatter() => formatterFactory.Invoke();

        public ZLoggerOptions UseFormatter(Func<IZLoggerFormatter> formatterFactory)
        {
            this.formatterFactory = formatterFactory;
            return this;
        }

        public ZLoggerOptions UsePlainTextFormatter()
        {
            return UsePlainTextFormatter(_ => { });
        }

        public ZLoggerOptions UsePlainTextFormatter(Action<PlainTextZLoggerFormatter> formatterConfigure)
        {
            UseFormatter(() =>
            {
                var formatter = new PlainTextZLoggerFormatter();
                formatterConfigure.Invoke(formatter);
                return formatter;
            });
            return this;
        }

        static IZLoggerFormatter DefaultFormatterFactory()
        {
            return new PlainTextZLoggerFormatter();
        }
    }
}
