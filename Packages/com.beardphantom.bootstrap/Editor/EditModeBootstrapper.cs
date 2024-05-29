using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class EditModeBootstrapper
    {
        #region Constructors

        static EditModeBootstrapper()
        {
            EditorApplication.delayCall += () => PerformBootstrapping().Forget();
        }

        #endregion

        #region Methods

        internal static async UniTaskVoid PerformBootstrapping()
        {
            using var _ = EditModePlayerLoopScope.Create();

            try
            {
                await UniTask.NextFrame();

                Cleanup();

                GameObject editModeServicesPrefab;
                if (EditorBootstrapSingleton.instance.TryGetActiveEditModeServices(out var editModeServices))
                {
                    editModeServicesPrefab = editModeServices.gameObject;
                }
                else
                {
                    editModeServicesPrefab = AssetDatabase
                        .FindAssets("t:Prefab")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
                        .FirstOrDefault(g => g.TryGetComponent<EditModeServices>(out var _));
                }

                if (editModeServicesPrefab == null)
                {
                    return;
                }

                editModeServicesPrefab.SetActive(false);
                var editModeServicesInstance = Object.Instantiate(editModeServicesPrefab);
                var editModeServicesCmp = editModeServicesInstance.GetComponent<EditModeServices>();
                editModeServicesCmp.SourcePrefab = editModeServicesPrefab;
                editModeServicesInstance.hideFlags = HideFlags.HideAndDontSave;
                editModeServicesInstance.name = editModeServicesPrefab.name;
                editModeServicesPrefab.SetActive(true);

                EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", "Creating edit mode services.", 0f);
                App.ServiceLocator = new ServiceLocator();
                var context = new BootstrapContext(EditorProgressBarProgress.Instance);
                await App.ServiceLocator.CreateAsync(context, editModeServicesInstance);
                context.Progress.Complete();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Edit/Bootstrap/Select Live Edit Mode Services")]
        private static void SelectLiveServicesObject()
        {
            var liveServices = Resources.FindObjectsOfTypeAll<EditModeServices>()
                .FirstOrDefault(s => !EditorUtility.IsPersistent(s));
            if (liveServices != null)
            {
                Selection.activeObject = liveServices;
            }
        }

        [MenuItem("Edit/Bootstrap/Bootstrap Edit Mode")]
        private static void PerformBootstrappingMenuItem()
        {
            PerformBootstrapping().Forget();
        }

        private static void Cleanup()
        {
            App.ServiceLocator?.Dispose();
            App.ServiceLocator = default;

            var liveServices = Resources.FindObjectsOfTypeAll<EditModeServices>();
            foreach (var liveService in liveServices)
            {
                var gObj = liveService.gameObject;
                if (BootstrapUtility.IsFromPrefab(gObj))
                {
                    continue;
                }

                Object.DestroyImmediate(gObj, false);
            }
        }

        #endregion
    }
}