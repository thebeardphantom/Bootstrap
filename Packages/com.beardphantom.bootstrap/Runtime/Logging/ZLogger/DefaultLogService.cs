#if BOOTSTRAP_ZLOGGER
using BeardPhantom.Bootstrap.SourceGen;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;
using ZLogger.Providers;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    [Serializable]
    [GenerateLogger]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public partial class DefaultLogService : IServiceWithCustomBindings, IServiceWithInitPriority, ILogService, IDisposable
    {
        private static int s_mainThreadId;

        private ILoggerFactory _loggerFactory;

        int IServiceWithInitPriority.InitPriority => -1000;

        [field: SerializeField]
        private LogLevel ConsoleMinLogLevel { get; set; } = LogLevel.Debug;

        [field: SerializeField]
        private LogLevel FileMinLogLevel { get; set; } = LogLevel.Trace;

        [field: SerializeField]
        private string FilePath { get; set; } = "Logs/App.log";

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public ILogger GetLogger(string category)
        {
            return _loggerFactory.CreateLogger(category);
        }

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

        protected virtual bool ShouldLogToConsole(LogLevel level)
        {
            return level >= ConsoleMinLogLevel;
        }

        protected virtual bool ShouldLogToFile(LogLevel level)
        {
            return level >= FileMinLogLevel;
        }

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

        protected virtual void SetPrefixFormatterThreadSafe(PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter(
                $"[{0:short}] [{1}] ",
                (in MessageTemplate template, in LogInfo info) =>
                {
                    template.Format(info.LogLevel, info.Category);
                });
        }

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

        protected virtual void ConfigureConsoleProvider(ILoggingBuilder builder)
        {
            builder.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(SetPrefixFormatter);
                    options.InternalErrorLogger = ex => Logging.Error($"ZLogger Error: {ex}");
                })
                .AddFilter<ZLoggerUnityDebugLoggerProvider>(ShouldLogToConsole);
        }

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

        void IDisposable.Dispose()
        {
            _loggerFactory?.Dispose();
            _loggerFactory = null;
        }

        void IService.InitService(BootstrapContext context)
        {
            s_mainThreadId = Thread.CurrentThread.ManagedThreadId;
            s_logger.ZLogTrace($"Test startup log.");
            Logging.LogHandler = BootstrapZLogHandler.Instance;
            _loggerFactory = CreateLoggerFactory();
            s_logger.ZLogInformation($"Log system setup complete.");
            FlushStartupLogs();
        }

        void IServiceWithCustomBindings.GetCustomBindings(List<Type> bindingTypes, out bool autoIncludeDeclaredType)
        {
            autoIncludeDeclaredType = true;
            bindingTypes.Add(typeof(ILogService));
        }
    }
}
#endif