using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using ZLogger.Providers;

namespace ZLogger
{
    public static class ZLoggerLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, bool consoleOutputEncodingToUtf8 = true)
        {
            if (consoleOutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }

            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerConsoleLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, Action<ZLoggerOptions> configure, bool consoleOutputEncodingToUtf8 = true)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLoggerConsole(consoleOutputEncodingToUtf8);
            builder.Services.Configure(configure);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string fileName)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerFileLoggerProvider>(x => new ZLoggerFileLoggerProvider(fileName, x.GetService<IOptions<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string fileName, Action<ZLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLoggerFile(fileName);
            builder.Services.Configure(configure);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(x => new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, x.GetService<IOptions<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(builder.Services);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLoggerRollingFile(fileNameSelector, timestampPattern, rollSizeKB);
            builder.Services.Configure(configure);

            return builder;
        }
    }
}