// #undef UNITY_EDITOR

using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Centralizes all of the ifdef checks for editor/build into one location to keep the rest of the codebase as clean as
    /// possible.
    /// </summary>
    public static partial class BootstrapUtility
    {
        public static void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            preHandler = PlayModeBootstrapHandler.Instance;
            postHandler = PlayModeBootstrapHandler.Instance;
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

        internal static RuntimeAppInstance GetRuntimeAppInstance()
        {
#if UNITY_EDITOR
            return new PlayModeAppInstance();
#else
            return new BuildAppInstance();
#endif
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

#if UNITY_EDITOR
    public static partial class BootstrapUtility
    {
        internal static bool IsFromPrefab(Object obj)
        {
            return PrefabUtility.IsPartOfAnyPrefab(obj) || IsPartOfPrefabStage(obj);
        }

        internal static bool IsPartOfPrefabStage(Object obj)
        {
            var gameObject = obj as GameObject;
            if (obj is Component cmp)
            {
                gameObject = cmp.gameObject;
            }

            if (gameObject == null)
            {
                return false;
            }

            PrefabStage stage = PrefabStageUtility.GetPrefabStage(gameObject);
            return stage != null;
        }

        internal static bool TryLoadEditModeState(out EditModeState editModeState)
        {
            string json = SessionState.GetString(EditModeAppInstance.EditModeStateSessionStateKey, null);
            if (string.IsNullOrWhiteSpace(json))
            {
                editModeState = null;
                return false;
            }

            editModeState = JsonConvert.DeserializeObject<EditModeState>(json);
            return editModeState != null;
        }

        static partial void ClearDirtyFlagInEditor(Object obj)
        {
            EditorUtility.ClearDirty(obj);
        }
    }
#endif
}