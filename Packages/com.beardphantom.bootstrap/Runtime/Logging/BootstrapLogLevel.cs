using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Aligns with LogLevel in Microsoft.Extensions.Logging.Abstractions
    /// </summary>
    public enum BootstrapLogLevel
    {
        /// <summary>
        /// Highly detailed messages used only for tracing execution during development.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Messages useful for debugging during development.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Messages tracking the general flow of the application.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Messages highlighting an unexpected or abnormal event that isn't otherwise an error.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Messages highlighting a failure in the current operation or request.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Messages highlighting a failure that requires immediate attention, such as data loss or a crash.
        /// </summary>
        Critical = 5,

        /// <summary>
        /// No messages should be logged.
        /// </summary>
        None = 6,
    }

    /// <summary>
    /// Extension methods for <see cref="BootstrapLogLevel"/>.
    /// </summary>
    public static class BootstrapLogLevelUtility
    {
        /// <summary>
        /// Converts a <see cref="BootstrapLogLevel"/> to the equivalent Unity <see cref="LogType"/>.
        /// </summary>
        /// <param name="logLevel">The log level to convert.</param>
        public static LogType GetUnityLogType(this BootstrapLogLevel logLevel)
        {
            return logLevel switch
            {
                BootstrapLogLevel.Trace or BootstrapLogLevel.Debug or BootstrapLogLevel.Information => LogType.Log,
                BootstrapLogLevel.Warning => LogType.Warning,
                BootstrapLogLevel.Error or BootstrapLogLevel.Critical => LogType.Error,
                _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
            };
        }
    }
}