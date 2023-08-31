using System.Diagnostics;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static class Log
    {
        #region Methods

        [Conditional("BOOTSTRAPPER_LOG_VERBOSE")]
        public static void Verbose(object message, Object context = default)
        {
            UnityEngine.Debug.unityLogger.Log("Bootstrapper", message, context);
        }

#if BOOTSTRAPPER_LOG_DISABLE
        [Conditional("FALSE")]
#endif
        public static void Info(object message, Object context = default)
        {
            UnityEngine.Debug.unityLogger.Log("Bootstrapper", message, context);
        }

        #endregion
    }
}