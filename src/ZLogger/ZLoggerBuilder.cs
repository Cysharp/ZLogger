using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ZLogger.Internal;
using ZLogger.Providers;

namespace ZLogger;

public static class ZLoggerBuilderExtensions
{
    public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder) => builder.AddZLoggerConsole((_, _) => { });
    public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, Action<ZLoggerConsoleOptions> configure) => builder.AddZLoggerConsole((options, _) => configure(options));
    public static ILoggingBuilder AddZLoggerConsole(this ILoggingBuilder builder, Action<ZLoggerConsoleOptions, IServiceProvider> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerConsoleOptions();
            configure(options, serviceProvider);

            if (options.ConfigureEnableAnsiEscapeCode)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    WindowsConsoleMode.TryEnableVirtualTerminalProcessing();
                }
            }
            if (options.OutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }

            return new ZLoggerConsoleLoggerProvider(options);
        });
        return builder;
    }

    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder) => builder.AddZLoggerInMemory(null, (_, _) => { }, _ => { });
    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder, Action<InMemoryObservableLogProcessor> configureProcessor) => builder.AddZLoggerInMemory(null, (_, _) => { }, configureProcessor);
    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder, Action<ZLoggerOptions> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => builder.AddZLoggerInMemory(null, configure, configureProcessor);
    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder, Action<ZLoggerOptions, IServiceProvider> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => builder.AddZLoggerInMemory(null, configure, configureProcessor);
    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder, object? processorKey, Action<ZLoggerOptions> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => builder.AddZLoggerInMemory(processorKey, (o, _) => configure(o), configureProcessor);
    public static ILoggingBuilder AddZLoggerInMemory(this ILoggingBuilder builder, object? processorKey, Action<ZLoggerOptions, IServiceProvider> configure, Action<InMemoryObservableLogProcessor> configureProcessor)
    {
        builder.Services.AddKeyedSingleton(processorKey, (_, _) => new InMemoryObservableLogProcessor());
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerInMemoryLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);

            var processor = serviceProvider.GetRequiredKeyedService<InMemoryObservableLogProcessor>(processorKey);
            processor.Formatter = options.CreateFormatter();
            configureProcessor(processor);

            return new ZLoggerInMemoryLoggerProvider(processor, options);
        });

        return builder;
    }

    public static ILoggingBuilder AddZLoggerLogProcessor(this ILoggingBuilder builder, IAsyncLogProcessor logProcessor) => builder.AddZLoggerLogProcessor((_, _) => logProcessor);
    public static ILoggingBuilder AddZLoggerLogProcessor(this ILoggingBuilder builder, Func<ZLoggerOptions, IAsyncLogProcessor> logProcessorFactory) => builder.AddZLoggerLogProcessor((o, _) => logProcessorFactory(o));
    public static ILoggingBuilder AddZLoggerLogProcessor(this ILoggingBuilder builder, Func<ZLoggerOptions, IServiceProvider, IAsyncLogProcessor> logProcessorFactory)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var processor = logProcessorFactory(options, serviceProvider);
            return new ZLoggerLogProcessorLoggerProvider(processor, options);
        });
        return builder;
    }

    public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream) => builder.AddZLoggerStream((_, _) => stream);
    public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream, Action<ZLoggerOptions> configure) => builder.AddZLoggerStream((o, _) => { configure(o); return stream; });
    public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Stream stream, Action<ZLoggerOptions, IServiceProvider> configure) => builder.AddZLoggerStream((o, p) => { configure(o, p); return stream; });
    public static ILoggingBuilder AddZLoggerStream(this ILoggingBuilder builder, Func<ZLoggerOptions, IServiceProvider, Stream> streamFactory)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var stream = streamFactory(options, serviceProvider);
            return new ZLoggerStreamLoggerProvider(stream, options);
        });
        return builder;
    }

    public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string filePath) => builder.AddZLoggerFile((_, _) => filePath);
    public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string filePath, Action<ZLoggerOptions> configure) => builder.AddZLoggerFile((o, _) => { configure(o); return filePath; });
    public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, string filePath, Action<ZLoggerOptions, IServiceProvider> configure) => builder.AddZLoggerFile((o, p) => { configure(o, p); return filePath; });
    public static ILoggingBuilder AddZLoggerFile(this ILoggingBuilder builder, Func<ZLoggerOptions, IServiceProvider, string> filePathFactory)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var filePath = filePathFactory(options, serviceProvider);
            return new ZLoggerFileLoggerProvider(filePath, options);
        });
        return builder;
    }

    /// <param name="filePathSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> filePathSelector, RollingInterval rollInterval, int rollSizeKB) => 
        builder.AddZLoggerRollingFile(filePathSelector, rollInterval, rollSizeKB, (_, _) => { });

    /// <param name="filePathSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> filePathSelector, RollingInterval rollInterval, int rollSizeKB, Action<ZLoggerOptions> configure) => 
        builder.AddZLoggerRollingFile(filePathSelector, rollInterval, rollSizeKB, (o, _) => configure(o));

    /// <param name="filePathSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public static ILoggingBuilder AddZLoggerRollingFile(this ILoggingBuilder builder, Func<DateTimeOffset, int, string> filePathSelector, RollingInterval rollInterval, int rollSizeKB, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        builder.Services.AddSingleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);
            return new ZLoggerRollingFileLoggerProvider(filePathSelector, rollInterval, rollSizeKB, options);
        });
        return builder;
    }

}
