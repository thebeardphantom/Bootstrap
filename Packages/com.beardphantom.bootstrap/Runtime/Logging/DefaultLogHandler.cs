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

        public static LogType GetLogType(in LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace or LogLevel.Debug or LogLevel.Information => LogType.Log,
                LogLevel.Warning => LogType.Warning,
                LogLevel.Error or LogLevel.Critical => LogType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }

        public static string GetFormatString(in LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
            {
                throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }

            var index = (int)logLevel;
            return s_logFormatStrings[index];
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, object message, Object context = null)
        {
            if (logLevel == LogLevel.None)
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