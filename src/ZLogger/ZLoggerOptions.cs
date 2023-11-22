using Microsoft.Extensions.Logging;
using ZLogger.Formatters;

namespace ZLogger
{
    public class ZLoggerOptions
    {
        public Action<Exception>? InternalErrorLogger { get; set; }
        public bool IncludeScopes { get; set; }
        public TimeProvider? TimeProvider { get; set; }
        public TimeSpan? FlushRate { get; set; }

        Func<IZLoggerFormatter> formatterFactory = DefaultFormatterFactory;

        public IZLoggerFormatter CreateFormatter() => formatterFactory.Invoke();

        public ZLoggerOptions UseFormatter(Func<IZLoggerFormatter> formatterFactory)
        {
            this.formatterFactory = formatterFactory;
            return this;
        }

        public ZLoggerOptions UsePlainTextFormatter(Action<PlainTextZLoggerFormatter>? configure)
        {
            UseFormatter(() =>
            {
                var formatter = new PlainTextZLoggerFormatter();
                configure?.Invoke(formatter);
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
