using ZLogger.Formatters;

namespace ZLogger
{
    /// <summary>
    /// ZLogger options.
    /// </summary>
    public class ZLoggerOptions
    {
        ///
        public Action<Exception>? InternalErrorLogger { get; set; }
        public bool IncludeScopes { get; set; }
        public TimeProvider? TimeProvider { get; set; }

        Func<IZLoggerFormatter> formatterFactory = DefaultFormatterFactory;

        /// <summary>
        /// Create formatter.
        /// </summary>
        /// <returns></returns>
        public IZLoggerFormatter CreateFormatter() => formatterFactory.Invoke();

        
        /// <summary>
        /// Set formatter factory.
        /// </summary>
        public ZLoggerOptions UseFormatter(Func<IZLoggerFormatter> formatterFactory)
        {
            this.formatterFactory = formatterFactory;
            return this;
        }

        /// <summary>
        /// Use JSON formatter.
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public ZLoggerOptions UsePlainTextFormatter(Action<PlainTextZLoggerFormatter>? configure = null)
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
