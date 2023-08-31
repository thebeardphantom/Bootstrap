using System.Diagnostics;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static class Debug
    {
        #region Methods

        [Conditional("BOOTSTRAPPER_LOG_VERBOSE")]
        public static void LogVerbose(object message, Object context = default)
        {
            UnityEngine.Debug.unityLogger.Log("Bootstrapper", message, context);
        }

#if BOOTSTRAPPER_LOG_DISABLE
        [Conditional("FALSE")]
#endif
        public static void Log(object message, Object context = default)
        {
            UnityEngine.Debug.unityLogger.Log("Bootstrapper", message, context);
        }

        #endregion
    }
}