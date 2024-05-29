using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

#if UNITY_EDITOR
namespace BeardPhantom.Bootstrap
{
    public static partial class BootstrapUtility
    {
        #region Methods

        public static bool IsFromPrefab(Object obj)
        {
            return PrefabUtility.IsPartOfAnyPrefab(obj) || IsPartOfPrefabStage(obj);
        }

        public static bool IsPartOfPrefabStage(Object obj)
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

            var stage = PrefabStageUtility.GetPrefabStage(gameObject);
            return stage != null;
        }

        public static T GetSourceObject<T>(T obj) where T : Component
        {
            var src = PrefabUtility.GetCorrespondingObjectFromSource(obj);
            if (src != null)
            {
                return src;
            }

            src = PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj);
            return src == null ? obj : src;
        }

        #endregion
    }
}
#endif