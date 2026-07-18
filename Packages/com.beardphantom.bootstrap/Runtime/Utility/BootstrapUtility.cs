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
        /// <summary>
        /// Checks whether <paramref name="obj"/> is a persistent asset (e.g. stored on disk) rather than a
        /// scene or runtime object. Always returns false outside of the editor.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        public static bool IsPersistent(Object obj)
        {
#if UNITY_EDITOR
            return EditorUtility.IsPersistent(obj);
#else
            return false;
#endif
        }

        /// <summary>
        /// Immediately destroys <paramref name="obj"/> and sets the reference to null.
        /// </summary>
        /// <typeparam name="T">The type of the Unity <see cref="Object"/>.</typeparam>
        /// <param name="obj">The reference to destroy and clear.</param>
        /// <param name="allowDestroyingAssets">If true, allows destroying persistent assets in the editor.</param>
        public static void DestroyReferenceImmediate<T>(ref T obj, bool allowDestroyingAssets = false) where T : Object
        {
            Object.DestroyImmediate(obj, allowDestroyingAssets);
            obj = null;
        }
    }
}