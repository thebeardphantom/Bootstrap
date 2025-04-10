using System.Runtime.CompilerServices;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class Logging
    {
        public const string BootstrapTag = "Bootstrap";

        public static readonly ILogHandler DefaultLogHandler = new DefaultLogHandler();

        public static ILogHandler LogHandler { get; set; } = DefaultLogHandler;

        public static BootstrapLogLevel MinLogLevel { get; set; } = BootstrapLogLevel.Information;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Trace(object message, Object context = null)
        {
            Log(BootstrapLogLevel.Trace, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Debug(object message, Object context = null)
        {
            Log(BootstrapLogLevel.Debug, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Info(object message, Object context = null)
        {
            Log(BootstrapLogLevel.Information, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Warn(object message, Object context = null)
        {
            Log(BootstrapLogLevel.Warning, message, context);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Error(object message, Object context = null)
        {
            Log(BootstrapLogLevel.Error, message, context);
        }

        private static void Log(BootstrapLogLevel logLevel, object message, Object context = null)
        {
            if (logLevel < MinLogLevel)
            {
                return;
            }

            LogHandler?.Log(logLevel, message, context);
        }
    }
}