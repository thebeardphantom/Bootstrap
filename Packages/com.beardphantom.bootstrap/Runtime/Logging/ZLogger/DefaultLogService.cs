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
    [Serializable]
    [GenerateLogger]
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public partial class DefaultLogService : IServiceWithCustomBindings, IServiceWithInitPriority, ILogService, IDisposable
    {
        private ILoggerFactory _loggerFactory;

        int IServiceWithInitPriority.InitPriority => -1000;

        [field: SerializeField]
        private LogLevel ConsoleMinLogLevel { get; set; } = LogLevel.Debug;

        [field: SerializeField]
        private LogLevel FileMinLogLevel { get; set; } = LogLevel.Trace;

        [field: SerializeField]
        private string FilePath { get; set; } = "Temp/log.txt";

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
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddZLoggerUnityDebug(options =>
                    {
                        options.UsePlainTextFormatter(SetPrefixFormatter);
                    })
                    .AddFilter<ZLoggerUnityDebugLoggerProvider>(level => level >= ConsoleMinLogLevel);
            });
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

        protected virtual void ConfigureFileProvider(ILoggingBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                return;
            }

            File.Delete(FilePath);
            builder.AddZLoggerFile(
                    FilePath,
                    options =>
                    {
                        options.UsePlainTextFormatter(SetPrefixFormatter);
                    })
                .AddFilter<ZLoggerFileLoggerProvider>(level => level >= FileMinLogLevel);
        }

        [HideInCallstack]
        protected virtual void FlushStartupLogs()
        {
            var startupLogsAppExtension = App.GetExtension<StartupLogsAppExtension>();
            startupLogsAppExtension.Flush(this);
        }

        void IDisposable.Dispose()
        {
            _loggerFactory?.Dispose();
            _loggerFactory = null;
        }

        void IService.InitService(BootstrapContext context)
        {
            Logging.LogHandler = BootstrapZLogHandler.Instance;
            s_logger.ZLogTrace($"Test startup log.");
            _loggerFactory = CreateLoggerFactory();
            s_logger.ZLogInformation($"Log system setup complete.");
            s_logger.ZLogDebug($"Begin flushing startup logs.");
            FlushStartupLogs();
            s_logger.ZLogDebug($"Finished flushing startup logs.");
        }

        void IServiceWithCustomBindings.GetCustomBindings(List<Type> bindingTypes, out bool autoIncludeDeclaredType)
        {
            autoIncludeDeclaredType = true;
            bindingTypes.Add(typeof(ILogService));
        }
    }
}
#endif