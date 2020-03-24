using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using System;

namespace ZLog
{
    public static class LoggingBuilderExtensions
    {
        public static ILoggingBuilder AddZLog(this ILoggingBuilder builder)
        {
            builder.AddConfiguration();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ZLogLoggerProvider>());
            LoggerProviderOptions.RegisterProviderOptions<ZLogOptions, ZLogLoggerProvider>(builder.Services);

            return builder;
        }

        public static ILoggingBuilder AddZLog(this ILoggingBuilder builder, Action<ZLogOptions> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            builder.AddZLog();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}