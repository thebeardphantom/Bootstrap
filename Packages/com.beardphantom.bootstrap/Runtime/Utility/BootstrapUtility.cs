// #undef UNITY_EDITOR

using System.Diagnostics;
using UnityEngine;
#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
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
            string json = SessionState.GetString(EditorBootstrapHandler.EditModeState, default);
            if (string.IsNullOrWhiteSpace(json))
            {
                editModeState = default;
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