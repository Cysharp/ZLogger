using ZLogger.Formatters;
using Microsoft.Extensions.Logging;

namespace ZLogger
{
    [Flags]
    public enum LogInfoProperties
    {
        // TODO: needs None?
        Timestamp = 1 << 0,
        LogLevel = 1 << 1,
        CategoryName = 1 << 2,
        EventIdValue = 1 << 3,
        EventIdName = 1 << 4,
        All = Timestamp | LogLevel | CategoryName | EventIdValue | EventIdName
    }

    public class ZLoggerOptions
    {
        public Action<Exception>? InternalErrorLogger { get; set; }
        public bool IncludeScopes { get; set; }


        // TODO:these options should move to formatter? provider?
        public TimeSpan? FlushRate { get; set; }
        public IKeyNameMutator? KeyNameMutator { get; set; }
        public LogLevel LogToErrorThreshold { get; set; } = LogLevel.None;
        public LogInfoProperties IncludeProperties { get; set; } = LogInfoProperties.Timestamp | LogInfoProperties.LogLevel | LogInfoProperties.CategoryName;

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
