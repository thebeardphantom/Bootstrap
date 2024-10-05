#if UNITY_EDITOR
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
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
}
#endif