#if BOOTSTRAP_ZLOGGER
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
    [SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
    public class DefaultLogService : MonoBehaviour, IMultiboundBootstrapService, IDisposable, ILogService
    {
        private static readonly ILogger s_logger = LogUtility.GetStaticLogger<DefaultLogService>();

        private ILoggerFactory _loggerFactory;

        [field: SerializeField]
        private LogLevel ConsoleMinLogLevel { get; set; } = LogLevel.Debug;

        [field: SerializeField]
        private LogLevel FileMinLogLevel { get; set; } = LogLevel.Trace;

        [field: SerializeField]
        private string FilePath { get; set; } = "Temp/log.txt";

        [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
        public virtual string GetLogLevelAbbreviated(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => "TRC",
                LogLevel.Debug => "DBG",
                LogLevel.Information => "INF",
                LogLevel.Warning => "WRN",
                LogLevel.Error => "ERR",
                LogLevel.Critical => "CRT",
                LogLevel.None => "NNE",
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public ILogger GetLogger(Type type)
        {
            return _loggerFactory.CreateLogger(type.Name);
        }

        public bool TryGetLogger(Type type, out ILogger logger)
        {
            if (_loggerFactory == null)
            {
                logger = null;
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
                        .SetMinimumLevel(LogLevel.Trace)
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
            _loggerFactory?.Dispose();
            _loggerFactory = null;
        }

        void IBootstrapService.InitService(BootstrapContext context)
        {
            _loggerFactory = CreateLoggerFactory();
            s_logger.ZLogInformation($"Log system setup complete.");
        }

        void IMultiboundBootstrapService.GetOverrideBindingTypes(List<Type> bindingTypes)
        {
            bindingTypes.Add(GetType());
            bindingTypes.Add(typeof(ILogService));
        }
    }
}
#endif