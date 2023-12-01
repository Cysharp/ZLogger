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
            processor.Formatter = options.CreateFormatter();
            configureProcessor(processor);

            return new ZLoggerInMemoryLoggerProvider(processor, options);
        });

        return this;
    }

    public ZLoggerBuilder AddLogProcessor(IAsyncLogProcessor logProcessor) => AddLogProcessor((_, _) => logProcessor);
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IAsyncLogProcessor> logProcessorFactory) => AddLogProcessor((o, _) => logProcessorFactory(o));
    public ZLoggerBuilder AddLogProcessor(Func<ZLoggerOptions, IServiceProvider, IAsyncLogProcessor> logProcessorFactory)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var processor = logProcessorFactory(options, serviceProvider);
            return new ZLoggerLogProcessorLoggerProvider(processor, options);
        });

        return this;
    }

    public ZLoggerBuilder AddStream(Stream stream) => AddStream((_, _) => stream);
    public ZLoggerBuilder AddStream(Stream stream, Action<ZLoggerOptions> configure) => AddStream((o, _) => { configure(o); return stream; });
    public ZLoggerBuilder AddStream(Stream stream, Action<ZLoggerOptions, IServiceProvider> configure) => AddStream((o, p) => { configure(o, p); return stream; });
    public ZLoggerBuilder AddStream(Func<ZLoggerOptions, IServiceProvider, Stream> streamFactory)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var stream = streamFactory(options, serviceProvider);
            return new ZLoggerStreamLoggerProvider(stream, options);
        });

        return this;
    }

    public ZLoggerBuilder AddFile(string fileName) => AddFile((_, _) => fileName);
    public ZLoggerBuilder AddFile(string fileName, Action<ZLoggerOptions> configure) => AddFile((o, _) => { configure(o); return fileName; });
    public ZLoggerBuilder AddFile(string fileName, Action<ZLoggerOptions, IServiceProvider> configure) => AddFile((o, p) => { configure(o, p); return fileName; });
    public ZLoggerBuilder AddFile(Func<ZLoggerOptions, IServiceProvider, string> fileNameFactory)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            var fileName = fileNameFactory(options, serviceProvider);
            return new ZLoggerFileLoggerProvider(fileName, options);
        });

        return this;
    }

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, RollingInterval rollInterval, int rollSizeKB) => AddRollingFile(fileNameSelector, rollInterval, rollSizeKB, (_, _) => { });

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, RollingInterval rollInterval, int rollSizeKB, Action<ZLoggerOptions> configure) => AddRollingFile(fileNameSelector, rollInterval, rollSizeKB, (o, _) => configure(o));

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="rollInterval">Interval to automatically rotate files</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, RollingInterval rollInterval, int rollSizeKB, Action<ZLoggerOptions, IServiceProvider> configure)
    {
        loggingBuilder.Services.AddSingleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(serviceProvider =>
        {
            var options = new ZLoggerOptions();
            configure(options, serviceProvider);
            return new ZLoggerRollingFileLoggerProvider(fileNameSelector, rollInterval, rollSizeKB, options);
        });

        return this;
    }

}
