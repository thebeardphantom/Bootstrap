#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
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

            var tform = transform;
            tform.SetParent(null, false);
            tform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            tform.SetAsFirstSibling();
        }

        partial void TryReplaceWithOverrideInstance()
        {
            var contents = InternalEditorUtility.LoadSerializedFileAndForget(EditorBootstrapHandler.TEMP_BOOTSTRAPPER_PATH);
            var overridePrefab = contents.OfType<GameObject>().SingleOrDefault();
            if (overridePrefab == null)
            {
                return;
            }

            Log.Info($"Loading custom bootstrapper from path '{EditorBootstrapHandler.TEMP_BOOTSTRAPPER_PATH}'.");
            var overrideInstance = Instantiate(overridePrefab);
            overrideInstance.name = overridePrefab.name;
            overrideInstance.GetComponent<Bootstrapper>()._isOverrideInstance = true;
            DestroyImmediate(gameObject);
        }

        partial void ClearDirtyFlag(GameObject servicesPrefab)
        {
            EditorUtility.ClearDirty(servicesPrefab);
        }

        #endregion
    }
}
#endif