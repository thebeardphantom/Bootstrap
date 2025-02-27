using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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
            if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            using var _ = EditModePlayerLoopScope.Create();

            int progressId = Progress.Start(
                $"Edit Mode Bootstrapping @ {DateTime.Now}",
                "Cleanup",
                Progress.Options.Indefinite | Progress.Options.Managed | Progress.Options.Sticky);
            try
            {
                var sw = Stopwatch.StartNew();
                Cleanup();

                EditModeServices servicesCmp = BootstrapEditorSettingsUtility.GetValue(
                    asset => asset.EditModeServices,
                    out SettingsScope definedScope);
                if (servicesCmp == null)
                {
                    sw.Stop();
                    Progress.Report(
                        progressId,
                        1f,
                        $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms. No edit mode services defined in {definedScope} scope.");
                    Progress.Finish(progressId);
                    return;
                }

                GameObject servicesPrefab = servicesCmp.gameObject;
                servicesPrefab.SetActive(false);
                GameObject servicesInstance = Object.Instantiate(servicesPrefab);
                var servicesInstanceCmp = servicesInstance.GetComponent<EditModeServices>();
                servicesInstanceCmp.SourcePrefab = servicesPrefab;
                servicesInstanceCmp.SourceComponent = servicesCmp;
                servicesInstance.name = $"{servicesPrefab.name} Instance";
                servicesPrefab.SetActive(true);
                EditorUtility.ClearDirty(servicesPrefab);

                App.Init();

                var context = new BootstrapContext(null, App.AsyncTaskScheduler);
                var description = $"Creating edit mode services from prefab '{servicesPrefab.name}' from {definedScope} scope.";
                Progress.Report(progressId, 1f, description);
                EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", description, 1f);
                App.ServiceLocator.Create(context, servicesInstance, HideFlags.HideAndDontSave);

                Log.Verbose($"Waiting for idle {nameof(AsyncTaskScheduler)}.");
                while (!App.AsyncTaskScheduler.IsIdle)
                {
                    await App.AsyncTaskScheduler.FlushQueueAsync();
                }

                EditorUtility.ClearProgressBar();
                App.BootstrapState = AppBootstrapState.Ready;
                sw.Stop();
                Progress.Report(
                    progressId,
                    1f,
                    $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms with instance '{servicesInstance.name}' from {definedScope} scope.");
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
            EditModeServices services = BootstrapEditorSettingsUtility.GetValue(asset => asset.EditModeServices);
            if (!TryGetServicesInstance(out EditModeServices instance) || services != instance.SourceComponent)
            {
                PerformBootstrapping().Forget();
            }
        }

        public static void UpdateScriptingDefinesIfNecessary()
        {
            BuildTargetGroup selectedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(selectedBuildTargetGroup);

            PlayerSettings.GetScriptingDefineSymbols(namedBuildTarget, out string[] defines);

            HashSet<string> set = defines.ToHashSet();
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

        public static void Cleanup()
        {
            App.DeinitializeIfInMode(AppInitMode.EditMode);

            EditModeServices[] liveServices = Resources.FindObjectsOfTypeAll<EditModeServices>();
            foreach (EditModeServices liveService in liveServices)
            {
                GameObject gObj = liveService.gameObject;
                if (BootstrapUtility.IsFromPrefab(gObj))
                {
                    continue;
                }

                Object.DestroyImmediate(gObj, false);
            }
        }

        private static void OnEditModeServicesChanged(EditModeServices obj)
        {
            PerformBootstrappingIfNecessary();
        }
    }
}