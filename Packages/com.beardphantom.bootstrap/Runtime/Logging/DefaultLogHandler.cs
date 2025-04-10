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
        public void Log(in BootstrapLogLevel logLevel, object message, Object context = null)
        {
            if (logLevel == BootstrapLogLevel.None)
            {
                return;
            }

            string formatString = GetFormatString(logLevel);
            message = string.Format(formatString, message);

            LogType logType = logLevel.GetUnityLogType();
            Debug.unityLogger.Log(logType, message, context);
        }
    }
}