using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Aligns with LogLevel in Microsoft.Extensions.Logging.Abstractions
    /// </summary>
    public enum BootstrapLogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
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