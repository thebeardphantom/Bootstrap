using System.Diagnostics;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class Log
    {
        public const string BOOTSTRAP_TAG = "Bootstrap";

        #region Methods

        public static readonly ILogHandler DefaultLogHandler = new DefaultLogHandler();

        public static ILogHandler LogHandler { get; set; } = DefaultLogHandler;

        [Conditional("BOOTSTRAPPER_LOG_VERBOSE")]
        internal static void Verbose(object message, Object context = default)
        {
            LogHandler?.Log(LogLevel.Verbose, BOOTSTRAP_TAG, message, context);
        }

#if BOOTSTRAPPER_LOG_DISABLE
        [Conditional("FALSE")]
#endif
        internal static void Info(object message, Object context = default)
        {
            LogHandler?.Log(LogLevel.Info, BOOTSTRAP_TAG, message, context);
        }

        #endregion
    }
}