#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;
using ZLogger.Providers;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class LogService : MonoBehaviour, IEarlyInitBootstrapService, IMultiboundBootstrapService, IDisposable, ILogService
    {
        private static readonly ILogger s_logger = LogUtility.GetStaticLogger<LogService>();

        private ILoggerFactory _loggerFactory;

        [field: SerializeField]
        private Microsoft.Extensions.Logging.LogLevel ConsoleMinLogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Debug;

        [field: SerializeField]
        private Microsoft.Extensions.Logging.LogLevel FileMinLogLevel { get; set; } = Microsoft.Extensions.Logging.LogLevel.Trace;

        [field: SerializeField]
        private string FilePath { get; set; } = "Temp/log.txt";

        private static string GetLogLevelAbbreviated(Microsoft.Extensions.Logging.LogLevel logLevel)
        {
            return logLevel switch
            {
                Microsoft.Extensions.Logging.LogLevel.Trace => "TRC",
                Microsoft.Extensions.Logging.LogLevel.Debug => "DBG",
                Microsoft.Extensions.Logging.LogLevel.Information => "INF",
                Microsoft.Extensions.Logging.LogLevel.Warning => "WRN",
                Microsoft.Extensions.Logging.LogLevel.Error => "ERR",
                Microsoft.Extensions.Logging.LogLevel.Critical => "CRT",
                Microsoft.Extensions.Logging.LogLevel.None => "NNE",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILogger GetLogger(Type type)
        {
            return _loggerFactory.CreateLogger(type.Name);
        }

        public bool TryGetLogger(Type type, out ILogger logger)
        {
            if (_loggerFactory == null)
            {
                logger = default;
                return false;
            }

            logger = GetLogger(type);
            return true;
        }

        protected virtual ILoggerFactory CreateLoggerFactory()
        {
            return LoggerFactory.Create(
                builder =>
                {
                    builder
                        .SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace)
                        .AddZLoggerUnityDebug(
                            options =>
                            {
                                options.UsePlainTextFormatter(SetPrefixFormatter);
                            })
                        .AddFilter<ZLoggerUnityDebugLoggerProvider>(level => level >= ConsoleMinLogLevel);
                    // ConfigureFileProvider(builder);
                });
        }

        protected virtual void SetPrefixFormatter(PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter(
                $"[{0}] [{1}] ",
                (in MessageTemplate template, in LogInfo info) => template.Format(GetLogLevelAbbreviated(info.LogLevel), info.Category));
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

        void IDisposable.Dispose()
        {
            _loggerFactory.Dispose();
            _loggerFactory = default;
        }

        /// <inheritdoc />
        Awaitable IEarlyInitBootstrapService.EarlyInitServiceAsync(BootstrapContext context)
        {
            _loggerFactory = CreateLoggerFactory();
            s_logger.ZLogInformation($"Log system setup complete.");
            return AwaitableUtility.GetCompleted();
        }

        /// <inheritdoc />
        Awaitable IBootstrapService.InitServiceAsync(BootstrapContext context)
        {
            return AwaitableUtility.GetCompleted();
        }

        void IMultiboundBootstrapService.GetExtraBindableTypes(List<Type> extraTypes)
        {
            extraTypes.Add(typeof(ILogService));
        }
    }
}
#endif