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

            Transform tform = transform;
            tform.SetParent(null, false);
            tform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            tform.SetAsFirstSibling();
        }

        partial void TryReplaceWithOverrideInstance()
        {
            Object[] contents = InternalEditorUtility.LoadSerializedFileAndForget(EditorBootstrapHandler.TempBootstrapperPath);
            GameObject overridePrefab = contents.OfType<GameObject>().SingleOrDefault();
            if (overridePrefab == null)
            {
                return;
            }

            Logging.Info($"Loading custom bootstrapper from path '{EditorBootstrapHandler.TempBootstrapperPath}'.");
            GameObject overrideInstance = Instantiate(overridePrefab);
            overrideInstance.name = overridePrefab.name;
            overrideInstance.GetComponent<Bootstrapper>()._isOverrideInstance = true;
            DestroyImmediate(gameObject);
        }
    }
}
#endif