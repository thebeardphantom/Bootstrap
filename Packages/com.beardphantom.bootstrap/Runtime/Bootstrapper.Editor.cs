#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper
    {
        #region Fields

        public const string BOOTSTRAP_GAMEOBJECT_NAME = "--BOOTSTRAP--";

        #endregion

        #region Methods

        private void Reset()
        {
            ResetGameObjectInHierarchy();
        }

        private void OnValidate()
        {
            ResetGameObjectInHierarchy();
        }

        private void ResetGameObjectInHierarchy()
        {
            if (PrefabUtility.IsPartOfPrefabAsset(this))
            {
                return;
            }

            if (PrefabStageUtility.GetPrefabStage(gameObject) != null)
            {
                return;
            }

            var tform = transform;
            tform.parent = null;
            tform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            tform.SetAsFirstSibling();
            tform.hideFlags = HideFlags.HideInInspector;
            name = BOOTSTRAP_GAMEOBJECT_NAME;
        }

        #endregion
    }
}
#endif