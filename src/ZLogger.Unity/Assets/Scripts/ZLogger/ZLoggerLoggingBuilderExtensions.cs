using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Runtime.InteropServices;
using ZLogger.Providers;

namespace ZLogger
{
    public static class ZLoggerLoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, bool consoleOutputEncodingToUtf8 = true, bool configureEnableAnsiEscapeCode = false)
        {
            if (configureEnableAnsiEscapeCode)
            {
                EnableAnsiEscapeCode();
            }

            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(x => new ZLoggerConsoleLoggerProvider(consoleOutputEncodingToUtf8, null, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerConsoleLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, Action<ZLoggerOptions> configure, bool consoleOutputEncodingToUtf8 = true, bool configureEnableAnsiEscapeCode = false)
        {
            if (configureEnableAnsiEscapeCode)
            {
                EnableAnsiEscapeCode();
            }

            return AddZLoggerConsole(builder, ZLoggerConsoleLoggerProvider.DefaultOptionName, configure, consoleOutputEncodingToUtf8);
        }

        public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, string optionName, Action<ZLoggerOptions> configure, bool consoleOutputEncodingToUtf8 = true, bool configureEnableAnsiEscapeCode = false)
        {
            if (configureEnableAnsiEscapeCode)
            {
                EnableAnsiEscapeCode();
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(x => new ZLoggerConsoleLoggerProvider(consoleOutputEncodingToUtf8, optionName, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerConsoleLoggerProvider>(builder.Services);

            builder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

            return builder;
        }

        static void EnableAnsiEscapeCode()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                WindowsConsoleMode.TryEnableVirtualTerminalProcessing();
            }
        }

        public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream)
        {
            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(x => new ZLoggerStreamLoggerProvider(stream, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream, Action<ZLoggerOptions> configure)
        {
            return AddZLoggerStream(builder, stream, ZLoggerStreamLoggerProvider.DefaultOptionName, configure);
        }

        public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream, string optionName, Action<ZLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }


            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(x => new ZLoggerStreamLoggerProvider(stream, optionName, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>(builder.Services);

            builder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerLogProcessor(this ILoggingBuilder builder, IAsyncLogProcessor logProcessor)
        {
            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(x => new ZLoggerLogProcessorLoggerProvider(logProcessor)));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string fileName)
        {
            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerFileLoggerProvider>(x => new ZLoggerFileLoggerProvider(fileName, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string fileName, Action<ZLoggerOptions> configure)
        {
            return AddZLoggerFile(builder, fileName, ZLoggerFileLoggerProvider.DefaultOptionName, configure);
        }

        public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string fileName, string optionName, Action<ZLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerFileLoggerProvider>(x => new ZLoggerFileLoggerProvider(fileName, optionName, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(builder.Services);

            builder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB)
        {
            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(x => new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(builder.Services);

            return builder;
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLoggerOptions> configure)
        {
            return AddZLoggerRollingFile(builder, fileNameSelector, timestampPattern, rollSizeKB, ZLoggerRollingFileLoggerProvider.DefaultOptionName, configure);
        }

        /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
        /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
        /// <param name="rollSizeKB">Limit size of single file.</param>
        public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, string optionName, Action<ZLoggerOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddConfiguration();
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(x => new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, optionName, x.GetService<IOptionsMonitor<ZLoggerOptions>>())));
            LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(builder.Services);

            builder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

            return builder;
        }
    }
}