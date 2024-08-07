﻿using System.Diagnostics;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class Log
    {
        public const string BootstrapTag = "Bootstrap";

        public const string VerboseLogDefine = "BOOTSTRAPPER_LOG_VERBOSE";

        public static readonly ILogHandler DefaultLogHandler = new DefaultLogHandler();

        public static ILogHandler LogHandler { get; set; } = DefaultLogHandler;

        [Conditional(VerboseLogDefine)]
        internal static void Verbose(object message, Object context = default)
        {
            LogHandler?.Log(LogLevel.Verbose, BootstrapTag, message, context);
        }

#if BOOTSTRAPPER_LOG_DISABLE
        [Conditional("FALSE")]
#endif
        internal static void Info(object message, Object context = default)
        {
            LogHandler?.Log(LogLevel.Info, BootstrapTag, message, context);
        }
    }
}