using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using Object = UnityEngine.Object;
using Progress = UnityEditor.Progress;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class EditModeBootstrapping
    {
        static EditModeBootstrapping()
        {
            EditorApplication.delayCall += () => PerformBootstrapping().Forget();
            BootstrapEditorProjectSettings.instance.EditModeServices.ValueChanged += OnEditModeServicesChanged;
            BootstrapEditorUserSettings.instance.EditModeServices.ValueChanged += OnEditModeServicesChanged;
        }

        public static async UniTaskVoid PerformBootstrapping()
        {
            if (EditorApplication.isCompiling)
            {
                return;
            }

            using var _ = EditModePlayerLoopScope.Create();

            var progressId = Progress.Start(
                $"Edit Mode Bootstrapping @ {DateTime.Now}",
                "Cleanup",
                Progress.Options.Indefinite | Progress.Options.Managed | Progress.Options.Sticky);
            try
            {
                var sw = Stopwatch.StartNew();
                Cleanup();

                var servicesCmp = BootstrapEditorSettingsUtility.GetValue(asset => asset.EditModeServices, out var definedScope);
                if (servicesCmp == null)
                {
                    sw.Stop();
                    Progress.Report(progressId, 1f, $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms. No edit mode services defined in {definedScope} scope.");
                    Progress.Finish(progressId);
                    return;
                }

                var servicesPrefab = servicesCmp.gameObject;
                servicesPrefab.SetActive(false);
                var servicesInstance = Object.Instantiate(servicesPrefab);
                var servicesInstanceCmp = servicesInstance.GetComponent<EditModeServices>();
                servicesInstanceCmp.SourcePrefab = servicesPrefab;
                servicesInstanceCmp.SourceComponent = servicesCmp;
                servicesInstance.name = $"{servicesPrefab.name} Instance";
                servicesPrefab.SetActive(true);
                EditorUtility.ClearDirty(servicesPrefab);

                App.ServiceLocator = new ServiceLocator();
                var context = new BootstrapContext(default);
                var description = $"Creating edit mode services from prefab '{servicesPrefab.name}' from {definedScope} scope.";
                Progress.Report(progressId, 1f, description);
                EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", description, 1f);
                await App.ServiceLocator.CreateAsync(context, servicesInstance, HideFlags.HideAndDontSave);
                EditorUtility.ClearProgressBar();
                App.BootstrapState = AppBootstrapState.Ready;
                sw.Stop();
                Progress.Report(progressId, 1f, $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms with instance '{servicesInstance.name}' from {definedScope} scope.");
                Progress.Finish(progressId);
            }
            catch (Exception ex)
            {
                Progress.Report(progressId, 1f, ex.ToString());
                Progress.Finish(progressId, Progress.Status.Failed);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static bool TryGetServicesInstance(out EditModeServices instance)
        {
            instance = Resources.FindObjectsOfTypeAll<EditModeServices>()
                .FirstOrDefault(s => !BootstrapUtility.IsFromPrefab(s));
            return instance != null;
        }

        public static void PerformBootstrappingIfNecessary()
        {
            var services = BootstrapEditorSettingsUtility.GetValue(asset => asset.EditModeServices);
            if (!TryGetServicesInstance(out var instance) || services != instance.SourceComponent)
            {
                PerformBootstrapping().Forget();
            }
        }

        public static void UpdateScriptingDefinesIfNecessary()
        {
            var selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(selectedBuildTargetGroup);

            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out var defines);

            var set = defines.ToHashSet();
            if (BootstrapEditorProjectSettings.instance.VerboseLogging)
            {
                set.Add(Log.VerboseLogDefine);
            }
            else
            {
                set.Remove(Log.VerboseLogDefine);
            }

            PlayerSettings.SetScriptingDefineSymbols(namedBuildTarget, set.ToArray());
        }

        private static void OnEditModeServicesChanged(EditModeServices obj)
        {
            PerformBootstrappingIfNecessary();
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

            App.BootstrapState = AppBootstrapState.None;
        }
    }
}