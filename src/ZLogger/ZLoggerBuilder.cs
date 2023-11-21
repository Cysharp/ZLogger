using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text;
using ZLogger.Internal;
using ZLogger.Providers;

namespace ZLogger;

public static class ZLoggerBuilderExtensions
{
    public static ILoggingBuilder AddZLogger(this ILoggingBuilder loggingBuilder, Action<ZLoggerBuilder> configure)
    {
        var builder = new ZLoggerBuilder(loggingBuilder);
        configure.Invoke(builder);
        return loggingBuilder;
    }
}

public class ZLoggerBuilder(ILoggingBuilder loggingBuilder)
{
    public ZLoggerBuilder AddConsole() => AddConsole((_, _) => { });
    public ZLoggerBuilder AddConsole(Action<ZLoggerConsoleOptions> configure) => AddConsole((options, _) => configure(options));
    public ZLoggerBuilder AddConsole(Action<ZLoggerConsoleOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(serviceProvider =>
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
        return this;
    }

    public ZLoggerBuilder AddInMemory(Action<InMemoryObservableLogProcessor> configureProcessor) => AddInMemory(null, (_, _) => { }, configureProcessor);
    // for IntelliSense, remove this overload
    // public ZLoggerBuilder AddInMemory(Action<ZLoggerOptions> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => AddInMemory(null, (o, _) => configure(o), configureProcessor);
    public ZLoggerBuilder AddInMemory(Action<ZLoggerOptions, IServiceProvider> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => AddInMemory(null, configure, configureProcessor);
    public ZLoggerBuilder AddInMemory(object? processorKey, Action<ZLoggerOptions> configure, Action<InMemoryObservableLogProcessor> configureProcessor) => AddInMemory(processorKey, (o, _) => configure(o), configureProcessor);
    public ZLoggerBuilder AddInMemory(object? processorKey, Action<ZLoggerOptions, IServiceProvider> configure, Action<InMemoryObservableLogProcessor> configureProcessor)
    {
        loggingBuilder.Services.AddKeyedSingleton(processorKey, (_, _) => new InMemoryObservableLogProcessor());
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerInMemoryLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);

            var processor = serviceProvider.GetRequiredKeyedService<InMemoryObservableLogProcessor>(processorKey);
            configureProcessor(processor);

            return new ZLoggerInMemoryLoggerProvider(processor, options);
        });

        return this;
    }

    public ZLoggerBuilder AddLogProcessor(IAsyncLogProcessor logProcessor) => AddLogProcessor((_, _) => logProcessor, (_, _) => { });
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IAsyncLogProcessor> logProcessorFactory) => AddLogProcessor((o, _) => logProcessorFactory(o), (_, _) => { });
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IServiceProvider, IAsyncLogProcessor> logProcessorFactory) => AddLogProcessor(logProcessorFactory, (_, _) => { });
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IAsyncLogProcessor> logProcessorFactory, Action<ZLoggerOptions> configure) => AddLogProcessor((o, _) => logProcessorFactory(o), (o, _) => configure(o));
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IServiceProvider, IAsyncLogProcessor> logProcessorFactory, Action<ZLoggerOptions> configure) => AddLogProcessor(logProcessorFactory, (o, _) => configure(o));
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IServiceProvider, IAsyncLogProcessor> logProcessorFactory, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);

            var processor = logProcessorFactory(options, serviceProvider);
            return new ZLoggerLogProcessorLoggerProvider(processor, options);
        });

        return this;
    }

    public ZLoggerBuilder AddStream(Stream stream) => AddStream((_, _) => stream, (_, _) => { });
    public ZLoggerBuilder AddStream(Stream stream, Action<ZLoggerOptions> configure) => AddStream((_, _) => stream, (o, _) => configure(o));
    public ZLoggerBuilder AddStream(Stream stream, Action<ZLoggerOptions, IServiceProvider> configure) => AddStream((_, _) => stream, configure);
    public ZLoggerBuilder AddStream(Func<ZLoggerOptions, IServiceProvider, Stream> streamFactory, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);

            var stream = streamFactory(options, serviceProvider);
            return new ZLoggerStreamLoggerProvider(stream, options);
        });

        return this;
    }

    public ZLoggerBuilder AddFile(string fileName) => AddFile((_, _) => fileName, (_, _) => { });
    public ZLoggerBuilder AddFile(string fileName, Action<ZLoggerOptions> configure) => AddFile((_, _) => fileName, (o, _) => configure(o));
    public ZLoggerBuilder AddFile(string fileName, Action<ZLoggerOptions, IServiceProvider> configure) => AddFile((_, _) => fileName, configure);
    public ZLoggerBuilder AddFile(Func<ZLoggerOptions, IServiceProvider, string> fileNameFactory, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);

            var fileName = fileNameFactory(options, serviceProvider);
            return new ZLoggerFileLoggerProvider(fileName, options);
        });

        return this;
    }

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB) => AddRollingFile(fileNameSelector, timestampPattern, rollSizeKB, (_, _) => { });

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLoggerOptions> configure) => AddRollingFile(fileNameSelector, timestampPattern, rollSizeKB, (o, _) => configure(o));

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);
            return new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, options);
        });

        return this;
    }

}
