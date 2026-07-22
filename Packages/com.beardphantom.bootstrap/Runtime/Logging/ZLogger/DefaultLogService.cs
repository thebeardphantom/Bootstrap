#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap.SourceGen;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;
using ZLogger.Providers;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// Default <see cref="ILogService"/> implementation. Configures ZLogger console and file logging providers
    /// and installs <see cref="BootstrapZLogHandler"/> as the active <see cref="ILogHandler"/>.
    /// </summary>
    [Serializable]
    [GenerateLogger]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public partial class DefaultLogService : IServiceWithCustomBindings, IServiceWithInitPriority, ILogService, IDisposable
    {
        private ILoggerFactory _loggerFactory;

        /// <inheritdoc />
        int IServiceWithInitPriority.InitPriority => -1000;

        /// <summary>
        /// The minimum level of messages written to the console provider.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The minimum level of messages written to the console provider.")]
        private LogLevel ConsoleMinLogLevel { get; set; } = LogLevel.Debug;

        /// <summary>
        /// The minimum level of messages written to the file provider.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The minimum level of messages written to the file provider.")]
        private LogLevel FileMinLogLevel { get; set; } = LogLevel.Trace;

        /// <summary>
        /// The path the file provider writes logs to.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The path the file provider writes logs to.")]
        private string FilePath { get; set; } = "Logs/App.log";

        /// <summary>
        /// Gets a logger for the given <paramref name="category"/>.
        /// </summary>
        /// <param name="category">The logger category name.</param>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public ILogger GetLogger(string category)
        {
            return _loggerFactory.CreateLogger(category);
        }

        /// <inheritdoc />
        public bool TryGetLogger(string category, out ILogger logger)
        {
            if (_loggerFactory.IsNull())
            {
                logger = null;
                return false;
            }

            logger = GetLogger(category);
            return true;
        }

        /// <summary>
        /// Creates the <see cref="ILoggerFactory"/> used by this service, configured with the console and file
        /// providers.
        /// </summary>
        protected virtual ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder
                    .ClearProviders()
                    .SetMinimumLevel(LogLevel.Trace);
                ConfigureConsoleProvider(builder);
                ConfigureFileProvider(builder);
            });
        }

        /// <summary>
        /// Determines whether a message at <paramref name="level"/> should be written to the console provider.
        /// </summary>
        /// <param name="level">The level of the message being logged.</param>
        protected virtual bool ShouldLogToConsole(LogLevel level)
        {
            return level >= ConsoleMinLogLevel;
        }

        /// <summary>
        /// Determines whether a message at <paramref name="level"/> should be written to the file provider.
        /// </summary>
        /// <param name="level">The level of the message being logged.</param>
        protected virtual bool ShouldLogToFile(LogLevel level)
        {
            return level >= FileMinLogLevel;
        }

        /// <summary>
        /// Configures the message prefix formatter used by the console provider.
        /// </summary>
        /// <param name="formatter">The formatter to configure.</param>
        protected virtual void SetPrefixFormatter(PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter(
                $"[{0:short}] [{1}] [{2}] ",
                (in MessageTemplate template, in LogInfo info) =>
                {
                    int frame = Time.frameCount;
                    template.Format(info.LogLevel, frame, info.Category);
                });
        }

        /// <summary>
        /// Configures the message prefix formatter used by the file provider. Thread-safe, since file writes can
        /// occur off the main thread.
        /// </summary>
        /// <param name="formatter">The formatter to configure.</param>
        protected virtual void SetPrefixFormatterThreadSafe(PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter(
                $"[{0:short}] [{1}] ",
                (in MessageTemplate template, in LogInfo info) =>
                {
                    template.Format(info.LogLevel, info.Category);
                });
        }

        /// <summary>
        /// Adds and configures the file logging provider on <paramref name="builder"/>, rotating the previous
        /// log file at <see cref="FilePath"/> if one exists.
        /// </summary>
        /// <param name="builder">The logging builder to configure.</param>
        protected virtual void ConfigureFileProvider(ILoggingBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return;
            }

            if (File.Exists(FilePath))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
                string extension = Path.GetExtension(FilePath);
                string directory = Path.GetDirectoryName(FilePath);
                string prevFileName = string.Concat(fileNameWithoutExtension, "_prev", extension);
                string prevFilePath = directory == null ? prevFileName : Path.Combine(directory, prevFileName);
                File.Copy(FilePath, prevFilePath, true);
                File.Delete(FilePath);
            }

            builder.AddZLoggerFile(
                    FilePath,
                    options =>
                    {
                        options.UsePlainTextFormatter(SetPrefixFormatterThreadSafe);
                        options.InternalErrorLogger = ex => Logging.Error($"ZLogger Error: {ex}");
                    })
                .AddFilter<ZLoggerFileLoggerProvider>(ShouldLogToFile);
        }

        /// <summary>
        /// Adds and configures the Unity console logging provider on <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The logging builder to configure.</param>
        protected virtual void ConfigureConsoleProvider(ILoggingBuilder builder)
        {
            builder.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(SetPrefixFormatter);
                    options.InternalErrorLogger = ex => Logging.Error($"ZLogger Error: {ex}");
                })
                .AddFilter<ZLoggerUnityDebugLoggerProvider>(ShouldLogToConsole);
        }

        /// <summary>
        /// Flushes any log entries queued by <see cref="StartupLogsAppExtension"/> before this service was
        /// available, if there are any.
        /// </summary>
        [HideInCallstack]
        protected virtual void FlushStartupLogs()
        {
            var startupLogsAppExtension = App.GetExtension<StartupLogsAppExtension>();
            if (!startupLogsAppExtension.HasStartupLogs)
            {
                return;
            }

            s_logger.ZLogDebug($"Begin flushing startup logs.");
            startupLogsAppExtension.Flush(this);
            s_logger.ZLogDebug($"Finished flushing startup logs.");
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            _loggerFactory?.Dispose();
            _loggerFactory = null;
        }

        /// <summary>
        /// Initializes the logging system: records the main thread id, installs <see cref="BootstrapZLogHandler"/>
        /// as the active <see cref="ILogHandler"/>, creates the logger factory, and flushes any queued startup logs.
        /// </summary>
        /// <param name="context">The context for this initialization.</param>
        void IService.InitService(BootstrapContext context)
        {
            s_logger.ZLogTrace($"Test startup log.");
            Logging.LogHandler = BootstrapZLogHandler.Instance;
            _loggerFactory = CreateLoggerFactory();
            s_logger.ZLogInformation($"Log system setup complete.");
            FlushStartupLogs();
        }

        /// <summary>
        /// Registers <see cref="ILogService"/> as an additional binding for this service.
        /// </summary>
        /// <param name="bindingTypes">The list of types to add bindings to.</param>
        /// <param name="autoIncludeDeclaredType">Whether the declared type should also be automatically bound.</param>
        void IServiceWithCustomBindings.GetCustomBindings(List<Type> bindingTypes, out bool autoIncludeDeclaredType)
        {
            autoIncludeDeclaredType = true;
            bindingTypes.Add(typeof(ILogService));
        }
    }
}
#endif