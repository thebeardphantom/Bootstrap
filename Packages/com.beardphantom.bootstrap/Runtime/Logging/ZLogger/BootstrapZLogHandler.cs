using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace BeardPhantom.Bootstrap.ZLogger
{
    /// <summary>
    /// <see cref="ILogHandler"/> implementation that routes log messages through ZLogger.
    /// </summary>
    public class BootstrapZLogHandler : ILogHandler
    {
        /// <summary>
        /// The singleton <see cref="BootstrapZLogHandler"/> instance.
        /// </summary>
        public static readonly BootstrapZLogHandler Instance = new();

        private static readonly ILogger s_logger = LogUtility.GetStaticLogger(Logging.BootstrapTag);

        private BootstrapZLogHandler() { }

        /// <inheritdoc />
        [HideInCallstack]
        public void Log(in BootstrapLogLevel logLevel, object message, Object context = null)
        {
            var microsoftLogLevel = (LogLevel)logLevel;
            s_logger.ZLog(microsoftLogLevel, $"{message}", context);
        }
    }
}