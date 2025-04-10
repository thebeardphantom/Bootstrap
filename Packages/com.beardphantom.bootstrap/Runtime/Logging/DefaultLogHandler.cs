using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    internal class DefaultLogHandler : ILogHandler
    {
        public const string LogFormatString = "[" + Logging.BootstrapTag + "] {0}";

        private static readonly string[] s_logFormatStrings =
        {
            $"[TRC] {LogFormatString}",
            $"[DBG] {LogFormatString}",
            $"[INF] {LogFormatString}",
            $"[WRN] {LogFormatString}",
            $"[ERR] {LogFormatString}",
            $"[CRT] {LogFormatString}",
        };

        public static LogType GetLogType(in BootstrapLogLevel logLevel)
        {
            return logLevel switch
            {
                BootstrapLogLevel.Trace or BootstrapLogLevel.Debug or BootstrapLogLevel.Information => LogType.Log,
                BootstrapLogLevel.Warning => LogType.Warning,
                BootstrapLogLevel.Error or BootstrapLogLevel.Critical => LogType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }

        public static string GetFormatString(in BootstrapLogLevel logLevel)
        {
            if (logLevel == BootstrapLogLevel.None)
            {
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }

            var index = (int)logLevel;
            return s_logFormatStrings[index];
        }

        /// <inheritdoc />
        public void Log(BootstrapLogLevel logLevel, object message, Object context = null)
        {
            if (logLevel == BootstrapLogLevel.None)
            {
                return;
            }

            string formatString = GetFormatString(logLevel);
            message = string.Format(formatString, message);

            LogType logType = GetLogType(logLevel);
            Debug.unityLogger.Log(logType, message, context);
        }
    }
}