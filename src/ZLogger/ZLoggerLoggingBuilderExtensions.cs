using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;
using System.Text;
using ZLogger.Internal;
using ZLogger.Providers;

namespace ZLogger;

public class ZLoggerBuilder(ILoggingBuilder loggingBuilder)
{
    public ZLoggerBuilder AddConsole()
    {
        return AddConsole(ZLoggerConsoleLoggerProvider.DefaultOptionName, static _ => { });
    }

    public ZLoggerBuilder AddConsole(Action<ZLoggerConsoleOptions> configure)
    {
        return AddConsole(ZLoggerConsoleLoggerProvider.DefaultOptionName, configure);
    }

    public ZLoggerBuilder AddConsole(string optionName, Action<ZLoggerConsoleOptions> configure)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerConsoleLoggerProvider>(x => new ZLoggerConsoleLoggerProvider(optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerConsoleOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerConsoleOptions, ZLoggerConsoleLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerConsoleOptions>(optionName).Configure(options =>
        {
            configure(options);
            
            if (options.ConfigureEnableAnsiEscapeCode)
            {
                EnableAnsiEscapeCode();
            }
            if (options.OutputEncodingToUtf8)
            {
                Console.OutputEncoding = new UTF8Encoding(false);
            }
        });

        return this;
    }

    static void EnableAnsiEscapeCode()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            WindowsConsoleMode.TryEnableVirtualTerminalProcessing();
        }
    }

    public ZLoggerBuilder AddStream(Stream stream)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(x => new ZLoggerStreamLoggerProvider(stream, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>(loggingBuilder.Services);

        return this;
    }

    public ZLoggerBuilder AddStream(Stream stream, Action<ZLoggerOptions> configure)
    {
        return AddStream(stream, ZLoggerStreamLoggerProvider.DefaultOptionName, configure);
    }

    public ZLoggerBuilder AddStream(Stream stream, string optionName, Action<ZLoggerOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }


        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerStreamLoggerProvider>(x => new ZLoggerStreamLoggerProvider(stream, optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

        return this;
    }

    public ZLoggerBuilder AddLogProcessor(IAsyncLogProcessor logProcessor)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(x => new ZLoggerLogProcessorLoggerProvider(logProcessor, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(loggingBuilder.Services);
        return this;
    }
        
    public ZLoggerBuilder AddLogProcessor(IAsyncLogProcessor logProcessor, Action<ZLoggerOptions> configure)
    {
        return AddLogProcessor(logProcessor, ZLoggerLogProcessorLoggerProvider.DefaultOptionName, configure);
    }
        
    public ZLoggerBuilder AddLogProcessor(IAsyncLogProcessor logProcessor, string optionName, Action<ZLoggerOptions> configure)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(x => new ZLoggerLogProcessorLoggerProvider(logProcessor, optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);
        return this;
    }

    public ZLoggerBuilder AddFile(string fileName)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerFileLoggerProvider>(x => new ZLoggerFileLoggerProvider(fileName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(loggingBuilder.Services);

        return this;
    }

    public ZLoggerBuilder AddFile(string fileName, Action<ZLoggerOptions> configure)
    {
        return AddFile(fileName, ZLoggerFileLoggerProvider.DefaultOptionName, configure);
    }

    public ZLoggerBuilder AddFile(string fileName, string optionName, Action<ZLoggerOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerFileLoggerProvider>(x => new ZLoggerFileLoggerProvider(fileName, optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

        return this;
    }

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(x => new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(loggingBuilder.Services);

        return this;
    }

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, Action<ZLoggerOptions> configure)
    {
        return AddRollingFile(fileNameSelector, timestampPattern, rollSizeKB, ZLoggerRollingFileLoggerProvider.DefaultOptionName, configure);
    }

    /// <param name="fileNameSelector">DateTimeOffset is date of file open time(UTC), int is number sequence.</param>
    /// <param name="timestampPattern">DateTimeOffset is write time of message(UTC). If pattern is different previously then roll new file.</param>
    /// <param name="rollSizeKB">Limit size of single file.</param>
    public ZLoggerBuilder AddRollingFile(Func<DateTimeOffset, int, string> fileNameSelector, Func<DateTimeOffset, DateTimeOffset> timestampPattern, int rollSizeKB, string optionName, Action<ZLoggerOptions> configure)
    {
        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerRollingFileLoggerProvider>(x => new ZLoggerRollingFileLoggerProvider(fileNameSelector, timestampPattern, rollSizeKB, optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);

        return this;
    }

    public ZLoggerBuilder AddInMemory()
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.AddSingleton<InMemoryObservableLogProcessor>();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(x => new ZLoggerLogProcessorLoggerProvider(x.GetRequiredService<InMemoryObservableLogProcessor>(), x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(loggingBuilder.Services);
        return this;
    }

    public ZLoggerBuilder AddInMemory(Action<ZLoggerOptions> configure)
    {
        return AddInMemory(ZLoggerLogProcessorLoggerProvider.DefaultOptionName, configure);
    }

    public ZLoggerBuilder AddInMemory(string optionName, Action<ZLoggerOptions> configure)
    {
        loggingBuilder.AddConfiguration();
        loggingBuilder.Services.AddSingleton<InMemoryObservableLogProcessor>();
        loggingBuilder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, ZLoggerLogProcessorLoggerProvider>(x => new ZLoggerLogProcessorLoggerProvider(x.GetRequiredService<InMemoryObservableLogProcessor>(), optionName, x.GetRequiredService<IOptionsMonitor<ZLoggerOptions>>())));
        LoggerProviderOptions.RegisterProviderOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(loggingBuilder.Services);

        loggingBuilder.Services.AddOptions<ZLoggerOptions>(optionName).Configure(configure);
        return this;
    }
}

public static class ZLoggerLoggingBuilderExtensions
{
    public static ILoggingBuilder AddZLogger(this ILoggingBuilder loggingBuilder, Action<ZLoggerBuilder> configure)
    {
        var builder = new ZLoggerBuilder(loggingBuilder);
        configure.Invoke(builder);
        return loggingBuilder;
    }
}
