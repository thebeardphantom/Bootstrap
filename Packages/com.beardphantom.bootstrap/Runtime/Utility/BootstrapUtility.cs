using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeardPhantom.Bootstrap
{
    public static partial class BootstrapUtility
    {
        public static void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            preHandler = EditorBootstrapHandler.Instance;
            postHandler = EditorBootstrapHandler.Instance;
#else
            preHandler = BuildBootstrapHandler.Instance;
            postHandler = BuildBootstrapHandler.Instance;
#endif
        }

        public static bool IsInPlayMode()
        {
#if UNITY_EDITOR
            return EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        public static void ClearDirtyFlag(Object obj)
        {
#if UNITY_EDITOR
            EditorUtility.ClearDirty(obj);
#endif
        }
    }
}