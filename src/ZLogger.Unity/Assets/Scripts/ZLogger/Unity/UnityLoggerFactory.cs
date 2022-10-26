#nullable disable

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZLogger.Providers;

namespace ZLogger
{
    public static class UnityLoggerFactory
    {
        public static ILoggerFactory Create(Action<ILoggingBuilder> configure)
        {
            var services = new ServiceCollection();

            services.AddLogging(x =>
            {
                AddBeforeConfiguration(x); // register singleton
                configure(x);
                AddAfterConfiguration(x); // remove and register enumerable
            });

            // use this for check IL2CPP type information.
            //var provider = new DiagnosticServiceProvider(services);

            var provider = services.BuildServiceProvider();

            var loggerFactory = provider.GetService<ILoggerFactory>();
            return new DisposingLoggerFactory(loggerFactory, provider);
        }

        static void AddBeforeConfiguration(ILoggingBuilder builder)
        {
            builder.Services.TryAddSingleton<ILoggerProviderConfigurationFactory, LoggerProviderConfigurationFactory>();
            builder.Services.TryAddSingleton(typeof(ILoggerProviderConfiguration<>), typeof(LoggerProviderConfiguration<>));
        }

        static void AddAfterConfiguration(ILoggingBuilder builder)
        {
            var removeTargets = builder.Services.Where(item => (
                        item.ServiceType == typeof(IConfigureOptions<ZLoggerOptions>)
                    && (item?.ImplementationType?.FullName?.StartsWith("Microsoft.Extensions.Logging.Configuration.LoggerProviderConfigureOptions") ?? false)))
                .ToArray();

            foreach (var item in removeTargets)
            {
                builder.Services.Remove(item);
            }

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerUnityLoggerProvider>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerConsoleLoggerProvider>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>>());
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<ZLoggerOptions>, LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>>());
        }

        // for IL2CPP.
        static void TypeHint()
        {
            _ = new LoggerFactory(default(IEnumerable<ILoggerProvider>), default(LoggerFilterOptions));
            {
                var setups = new IConfigureOptions<ZLoggerOptions>[] { new ConfigureOptions<ZLoggerOptions>(default) }.AsEnumerable();
                var postConfigure = new IPostConfigureOptions<ZLoggerOptions>[] { new PostConfigureOptions<ZLoggerOptions>(default, default) }.AsEnumerable();
                var optionFactory = new OptionsFactory<ZLoggerOptions>(setups, postConfigure);
                _ = new OptionsManager<ZLoggerOptions>(optionFactory);

                var sources = new[] { new ConfigurationChangeTokenSource<ZLoggerOptions>(default) }.AsEnumerable();
                var cache = new OptionsCache<ZLoggerOptions>();
                _ = new OptionsMonitor<ZLoggerOptions>(optionFactory, sources, cache);
            }
            {
                var setups = new IConfigureOptions<LoggerFilterOptions>[] { new ConfigureOptions<LoggerFilterOptions>(default) }.AsEnumerable();
                var postConfigure = new IPostConfigureOptions<LoggerFilterOptions>[] { new PostConfigureOptions<LoggerFilterOptions>(default, default) }.AsEnumerable();
                var optionFactory = new OptionsFactory<LoggerFilterOptions>(setups, postConfigure);
                _ = new OptionsManager<LoggerFilterOptions>(optionFactory);

                var sources = new[] { new ConfigurationChangeTokenSource<LoggerFilterOptions>(default) }.AsEnumerable();
                var cache = new OptionsCache<LoggerFilterOptions>();
                _ = new OptionsMonitor<LoggerFilterOptions>(optionFactory, sources, cache);
            }
            {
                _ = Options.Create<LoggerFilterOptions>(new LoggerFilterOptions());
                _ = Options.Create<ZLoggerOptions>(new ZLoggerOptions());
                _ = new ConfigureNamedOptions<LoggerFilterOptions>(default, default);
                _ = new ConfigureNamedOptions<ZLoggerOptions>(default, default);
            }

            var loggingConfigurations = new[] { new LoggingConfiguration(default) }.AsEnumerable();
            var loggingProviderConfigurationFactory = new LoggerProviderConfigurationFactory(loggingConfigurations);

            {
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerUnityLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerUnityLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerUnityLoggerProvider>(providerConfiguration);
                }
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerConsoleLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerConsoleLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerConsoleLoggerProvider>(providerConfiguration);
                }
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerFileLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerFileLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerFileLoggerProvider>(providerConfiguration);
                }
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerRollingFileLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerRollingFileLoggerProvider>(providerConfiguration);
                }
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerStreamLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerStreamLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerStreamLoggerProvider>(providerConfiguration);
                }
                {
                    var providerConfiguration = new LoggerProviderConfiguration<ZLoggerLogProcessorLoggerProvider>(loggingProviderConfigurationFactory);
                    _ = new LoggerProviderConfigureOptions<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(providerConfiguration);
                    _ = new LoggerProviderOptionsChangeTokenSource<ZLoggerOptions, ZLoggerLogProcessorLoggerProvider>(providerConfiguration);
                }
            }
        }

        class LoggingConfiguration
        {
            public IConfiguration Configuration { get; }

            public LoggingConfiguration(IConfiguration configuration)
            {
                Configuration = configuration;
            }
        }

        class LoggerProviderConfiguration<T> : ILoggerProviderConfiguration<T>
        {
            public LoggerProviderConfiguration(ILoggerProviderConfigurationFactory providerConfigurationFactory)
            {
                Configuration = providerConfigurationFactory.GetConfiguration(typeof(T));
            }

            public IConfiguration Configuration { get; }
        }

        class LoggerProviderConfigurationFactory : ILoggerProviderConfigurationFactory
        {
            private readonly IEnumerable<LoggingConfiguration> _configurations;

            public LoggerProviderConfigurationFactory(IEnumerable<LoggingConfiguration> configurations)
            {
                _configurations = configurations;
            }

            public IConfiguration GetConfiguration(Type providerType)
            {
                if (providerType == null)
                {
                    throw new ArgumentNullException(nameof(providerType));
                }

                var fullName = providerType.FullName;
                var alias = ProviderAliasUtilities.GetAlias(providerType);
                var configurationBuilder = new ConfigurationBuilder();
                foreach (var configuration in _configurations)
                {
                    var sectionFromFullName = configuration.Configuration.GetSection(fullName);
                    configurationBuilder.AddConfiguration(sectionFromFullName);

                    if (!string.IsNullOrWhiteSpace(alias))
                    {
                        var sectionFromAlias = configuration.Configuration.GetSection(alias);
                        configurationBuilder.AddConfiguration(sectionFromAlias);
                    }
                }
                return configurationBuilder.Build();
            }
        }

        class LoggerProviderConfigureOptions<TOptions, TProvider> : ConfigureFromConfigurationOptions<TOptions> where TOptions : class
        {
            public LoggerProviderConfigureOptions(ILoggerProviderConfiguration<TProvider> providerConfiguration)
                : base(providerConfiguration.Configuration)
            {
            }
        }

        static class ProviderAliasUtilities
        {
            private const string AliasAttibuteTypeFullName = "Microsoft.Extensions.Logging.ProviderAliasAttribute";
            private const string AliasAttibuteAliasProperty = "Alias";

            internal static string GetAlias(Type providerType)
            {
                foreach (var attribute in providerType.GetTypeInfo().GetCustomAttributes(inherit: false))
                {
                    if (attribute.GetType().FullName == AliasAttibuteTypeFullName)
                    {
                        var valueProperty = attribute
                            .GetType()
                            .GetProperty(AliasAttibuteAliasProperty, BindingFlags.Public | BindingFlags.Instance);

                        if (valueProperty != null)
                        {
                            return valueProperty.GetValue(attribute) as string;
                        }
                    }
                }

                return null;
            }
        }

        class DisposingLoggerFactory : ILoggerFactory, IDisposable
        {
            readonly ILoggerFactory loggerFactory;
            readonly IServiceProvider serviceProvider;

            public DisposingLoggerFactory(ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
            {
                if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
                if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

                this.loggerFactory = loggerFactory;
                this.serviceProvider = serviceProvider;
            }

            public void Dispose()
            {
                (serviceProvider as IDisposable)?.Dispose();
            }

            public ILogger CreateLogger(string categoryName)
            {
                return loggerFactory.CreateLogger(categoryName);
            }

            public void AddProvider(ILoggerProvider provider)
            {
                loggerFactory.AddProvider(provider);
            }
        }
    }
}
