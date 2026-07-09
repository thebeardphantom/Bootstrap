// #undef UNITY_EDITOR

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Centralizes all of the ifdef checks for editor/build into one location to keep the rest of the codebase as clean as
    /// possible.
    /// </summary>
    public static class BootstrapUtility
    {
        public static bool IsPersistent(Object obj)
        {
#if UNITY_EDITOR
            return EditorUtility.IsPersistent(obj);
#else
            return false;
#endif
        }

        public static void DestroyReferenceImmediate<T>(ref T obj, bool allowDestroyingAssets = false) where T : Object
        {
            Object.DestroyImmediate(obj, allowDestroyingAssets);
            obj = null;
        }
    }
}