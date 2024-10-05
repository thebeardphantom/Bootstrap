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

        public static GameObject InstantiateAsInactive(GameObject prefab)
        {
            bool isActive = prefab.activeSelf;
            GameObject instance;
            if (isActive)
            {
                prefab.SetActive(false);
                instance = Object.Instantiate(prefab);
                prefab.SetActive(true);
                ClearDirtyFlag(prefab);
            }
            else
            {
                instance = Object.Instantiate(prefab);
            }

            return instance;
        }

        internal static bool IsInPlayMode()
        {
#if UNITY_EDITOR
            return EditorApplication.isPlayingOrWillChangePlaymode;
#else
            return true;
#endif
        }

        [Conditional("UNITY_EDITOR")]
        internal static void ClearDirtyFlag(Object obj)
        {
            ClearDirtyFlagInEditor(obj);
        }

        static partial void ClearDirtyFlagInEditor(Object obj);
    }
}