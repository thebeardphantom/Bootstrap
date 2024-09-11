#if BOOTSTRAP_ZLOGGER
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using UnityEngine;
using ZLogger;
using ZLogger.Formatters;
using ZLogger.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class LogService : MonoBehaviour, IEarlyInitBootstrapService, IDisposable, ILogService
    {
        private static readonly LogFile[] s_logFiles =
        {
            new("Temp/log_all.txt"),
            new("Temp/log_debug.txt"),
        };

        private static readonly ILogger s_logger = LogUtility.GetStaticLogger<LogService>();

        private ILoggerFactory _loggerFactory;

        [field: SerializeField]
        private Microsoft.Extensions.Logging.LogLevel MinLogLevel { get; set; }

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
                        .SetMinimumLevel(MinLogLevel)
                        .AddZLoggerUnityDebug(
                            options =>
                            {
                                options.UsePlainTextFormatter(SetPrefixFormatter);
                            });
                    foreach (LogFile logFile in s_logFiles)
                    {
                        logFile.Delete();
                        logFile.AddFileSink(builder);
                    }
                });
        }

        protected virtual void SetPrefixFormatter(PlainTextZLoggerFormatter formatter)
        {
            formatter.SetPrefixFormatter(
                $"[{0}] [{1}] ",
                (in MessageTemplate template, in LogInfo info) => template.Format(GetLogLevelAbbreviated(info.LogLevel), info.Category));
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

        private readonly struct LogFile
        {
            private readonly string _path;

            public LogFile(string path)
            {
                _path = path;
            }

            public void Delete()
            {
                if (File.Exists(_path))
                {
                    File.Delete(_path);
                }
            }

            public void AddFileSink(ILoggingBuilder loggingBuilder)
            {
                loggingBuilder.AddZLoggerFile(_path);
            }
        }
    }
}
#endif