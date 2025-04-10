using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using System;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class EditModeBootstrapping
    {
        private const string EditModeServicesIIdKey = "EditModeServicesIId";

        public static Action BootstrappingComplete;

        static EditModeBootstrapping()
        {
            EditorApplication.delayCall += () => PerformBootstrappingAsync().Forget();
            BootstrapEditorProjectSettings.instance.EditModeEnvironment.ValueChanged += OnEditModeServicesChanged;
            BootstrapEditorUserSettings.instance.EditModeEnvironment.ValueChanged += OnEditModeServicesChanged;
        }

        public static async Awaitable PerformBootstrappingAsync()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            using var _ = EditModeAwaitableSupportScope.Create();

            int progressId = Progress.Start(
                $"Edit Mode Bootstrapping @ {DateTime.Now}",
                "Cleanup",
                Progress.Options.Indefinite | Progress.Options.Managed | Progress.Options.Sticky);
            try
            {
                var sw = Stopwatch.StartNew();
                Cleanup();

                EditModeBootstrapEnvironmentAsset editModeEnvironment = BootstrapEditorSettingsUtility.GetValue(
                    asset => asset.EditModeEnvironment,
                    out SettingsScope definedScope);
                if (editModeEnvironment == null)
                {
                    sw.Stop();
                    Progress.Report(
                        progressId,
                        1f,
                        $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms. No edit mode services defined in {definedScope} scope.");
                    Progress.Finish(progressId);
                    BootstrappingComplete?.Invoke();
                    return;
                }

                if (editModeEnvironment.IsNoOp)
                {
                    Progress.Report(progressId, 1f, "Edit Mode Environment does not have a services prefab.");
                    Progress.Finish(progressId, Progress.Status.Failed);
                }

                GameObject servicesInstance = editModeEnvironment.StartEnvironment();
                var editModeServicesInstance = servicesInstance.AddComponent<EditModeServicesInstance>();
                editModeServicesInstance.SourcePrefab = editModeEnvironment.ServicesPrefab;
                SessionState.SetInt(EditModeServicesIIdKey, editModeServicesInstance.GetInstanceID());
                App.Init();

                var context = new BootstrapContext(null, App.AsyncTaskScheduler);
                var description = $"Creating edit mode services from prefab '{editModeEnvironment.ServicesPrefab.name}' from {definedScope} scope.";
                Progress.Report(progressId, 1f, description);
                EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", description, 1f);
                App.ServiceLocator.Create(context, servicesInstance, HideFlags.HideAndDontSave);

                Logging.Trace($"Waiting for idle {nameof(AsyncTaskScheduler)}.");
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
                BootstrappingComplete?.Invoke();
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

        public static bool TryGetServicesInstance(out EditModeServicesInstance instance)
        {
            int instanceId = SessionState.GetInt(EditModeServicesIIdKey, 0);
            if (!Resources.InstanceIDIsValid(instanceId))
            {
                instance = default;
                return false;
            }

            instance = Resources.InstanceIDToObject(instanceId) as EditModeServicesInstance;
            return instance != null;
        }

        public static void PerformBootstrappingIfNecessary()
        {
            EditModeBootstrapEnvironmentAsset env = BootstrapEditorSettingsUtility.GetValue(asset => asset.EditModeEnvironment);

            /*
             * Bootstrap if:
             *     1. There's no existing instance already.
             *     2. The environment was cleared. PerformBootstrappingAsync will cleanup any existing instances.
             *     3. The prefab has changed.
             */
            bool hasExistingInstance = TryGetServicesInstance(out EditModeServicesInstance instance);
            if (!hasExistingInstance || env == null || env.ServicesPrefab != instance.SourcePrefab)
            {
                PerformBootstrappingAsync().Forget();
            }
        }

        public static void UpdateLogLevelIfNecessary()
        {
            BootstrapLogLevel minLogLevel = BootstrapEditorSettingsUtility.GetValue(asset => asset.MinLogLevel);
            Logging.MinLogLevel = minLogLevel;
        }

        public static void Cleanup()
        {
            App.DeinitializeIfInMode(AppInitMode.EditMode);

            if (TryGetServicesInstance(out EditModeServicesInstance servicesInstance))
            {
                Object.DestroyImmediate(servicesInstance, false);
            }

            SessionState.EraseInt(EditModeServicesIIdKey);
        }

        private static void OnEditModeServicesChanged(EditModeBootstrapEnvironmentAsset obj)
        {
            PerformBootstrappingIfNecessary();
        }
    }
}
