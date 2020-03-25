using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using ZLog.Providers;

namespace ZLog
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZLogConsole(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLogConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<ZLogOptions, ZLogConsoleLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLogConsole(this ILoggingBuilder builder, Action<ZLogOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLogConsole();
            builder.Services.Configure(configure);

            return builder;
        }

        public static ILoggingBuilder AddZLogFile(this ILoggingBuilder builder, string fileName)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLogFileLoggerProvider>(x => new ZLogFileLoggerProvider(fileName, x.GetService<IOptions<ZLogOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLogOptions, ZLogFileLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLogFile(this ILoggingBuilder builder, string fileName, Action<ZLogOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLogFile(fileName);
            builder.Services.Configure(configure);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLogRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLogRollingFileLoggerProvider>(x => new ZLogRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, x.GetService<IOptions<ZLogOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLogOptions, ZLogRollingFileLoggerProvider>(builder.Services);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLogRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLogOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLogRollingFile(fileNameSelector, timestampPattern, rollSizeKB);
            builder.Services.Configure(configure);

            return builder;
        }
    }
}