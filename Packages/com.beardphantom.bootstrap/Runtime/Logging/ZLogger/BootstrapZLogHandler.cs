using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    public class BootstrapZLogHandler : ILogHandler
    {
        public static readonly BootstrapZLogHandler Instance = new();

        private static readonly ILogger s_logger = LogUtility.GetStaticLogger(Logging.BootstrapTag);

        private BootstrapZLogHandler() { }

        public void Log(in BootstrapLogLevel logLevel, object message, Object context = null)
        {
            var microsoftLogLevel = (LogLevel)logLevel;
            s_logger.ZLog(microsoftLogLevel, $"{message}", context);
        }
    }
}