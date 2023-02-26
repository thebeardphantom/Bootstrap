#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper
    {
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

            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetAsFirstSibling();
            name = "--BOOTSTRAP--";
        }

        #endregion
    }
}
#endif