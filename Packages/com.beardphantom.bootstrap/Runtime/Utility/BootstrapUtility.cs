// #undef UNITY_EDITOR

using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
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
        /// Checks whether <paramref name="obj" /> is a persistent asset (e.g. stored on disk) rather than a
        /// scene or runtime object. Always returns false outside of the editor.
        /// </summary>
        public static bool IsPersistent(Object obj)
        {
#if UNITY_EDITOR
            return EditorUtility.IsPersistent(obj);
#else
            return false;
#endif
        }

        /// <summary>
        /// Immediately destroys <paramref name="obj" /> and sets the reference to null.
        /// </summary>
        public static void DestroyReferenceImmediate<T>(ref T obj, bool allowDestroyingAssets = false) where T : Object
        {
            Object.DestroyImmediate(obj, allowDestroyingAssets);
            obj = null;
        }

        public static bool IsTestRunnerScene(in Scene scene)
        {
            if (!scene.path.StartsWith("Assets/InitTestScene"))
            {
                return false;
            }

            var guidString = scene.name["InitTestScene".Length..];
            return Guid.TryParse(guidString, out _);
        }

        public static string GetFrameCountString()
        {
            return App.IsMainThread ? Time.frameCount.ToString() : "-";
        }
    }
}