#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.EditMode
{
    public class EditModeAppInstance : AppInstance
    {
        public const string EditModeServicesEntityIdSessionStateKey = "EditModeServicesIId";

        public const string EditModeStateSessionStateKey = "EDIT_MODE_STATE";

        private ServiceList _editModeServiceListInstance;

        public override ServiceList ActiveServiceList => _editModeServiceListInstance;

        public override bool IsQuitting => false;

        public EditModeAppInstance()
        {
            Logging.LogHandler = Logging.DefaultLogHandler;
            BootstrapEditorProjectSettings.instance.EditModeServices.ValueChanged += OnEditModeServicesChanged;
            BootstrapEditorUserSettings.instance.EditModeServices.ValueChanged += OnEditModeServicesChanged;
            if (BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
            {
                EditorSceneManager.playModeStartScene = null;
            }
        }

        private static bool TryFindActiveSceneEnvironment(out BootstrapEnvironmentAsset environment)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            MappedEnvironmentCollection<SceneAsset> sceneEnvironments =
                BootstrapEditorSettingsUtility.GetValue(a => a.EditorSceneEnvironments);
            return sceneEnvironments.TryFindEnvironmentForKey(sceneAsset, out environment);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_editModeServiceListInstance.IsNotNull())
            {
                Object.DestroyImmediate(_editModeServiceListInstance);
            }
        }

        public void ReinitializeIfNecessary()
        {
            ServiceList serviceList = BootstrapEditorSettingsUtility.GetValue(asset => asset.EditModeServices);

            /*
             * Bootstrap if:
             *     1. There's no existing instance already.
             *     2. The environment was cleared. PerformBootstrappingAsync will cleanup any existing instances.
             *     3. The services list asset has changed.
             */
            if (_editModeServiceListInstance.IsNull()
                || serviceList.IsNull()
                || serviceList != _editModeServiceListInstance.Source)
            {
                App.Quit();
                App.Initialize<EditModeAppInstance>();
            }
        }

        internal void OnExitingEditMode()
        {
            SessionState.EraseString(EditModeStateSessionStateKey);

            if (BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
            {
                if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Bootstrapper does not support untitled scenes."))
                {
                    EditorApplication.isPlaying = false;
                    return;
                }

                if (!TryFindActiveSceneEnvironment(out BootstrapEnvironmentAsset environment))
                {
                    environment = BootstrapEditorSettingsUtility.GetValue(a => a.DefaultPlayModeEnvironment);
                }

                if (environment.IsNotNull())
                {
                    EditorSceneManager.playModeStartScene = null;
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                    Logging.Trace("BootstrapEditorHelper prepping for playmode.");
                    EditorBuildSettingsScene bootstrapScene =
                        EditorBuildSettings.scenes.FirstOrDefault(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path));
                    if (bootstrapScene.IsNull())
                    {
                        Logging.Info("No valid first scene in EditorBuildSettings");
                        return;
                    }

                    var bootstrapSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootstrapScene.path);
                    EditorSceneManager.playModeStartScene = bootstrapSceneAsset;

                    using (ListPool<string>.Get(out List<string> scenePaths))
                    {
                        for (var i = 0; i < SceneManager.sceneCount; i++)
                        {
                            Scene scene = SceneManager.GetSceneAt(i);
                            if (scene.buildIndex != 0 && scene.IsValid())
                            {
                                scenePaths.Add(scene.path);
                            }
                        }

                        SelectedObjectPath[] selectedObjectPaths = Selection.gameObjects
                            .Where(g => g && g.scene.IsValid())
                            .Select(SelectedObjectPath.CreateInstance)
                            .ToArray();
                        var editModeState = new EditModeState
                        {
                            Environment = environment,
                            LoadedScenes = scenePaths,
                            SelectedObjects = selectedObjectPaths,
                        };
                        string editModeStateJson = JsonConvert.SerializeObject(editModeState);
                        SessionState.SetString(EditModeStateSessionStateKey, editModeStateJson);
                    }
                }
            }
        }

        internal override async Awaitable BootstrapAsync()
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            await base.BootstrapAsync();

            using var _ = EditModeAwaitableSupportScope.Create();

            int progressId = Progress.Start(
                $"Edit Mode Bootstrapping @ {DateTime.Now}",
                "Cleanup",
                Progress.Options.Indefinite | Progress.Options.Managed | Progress.Options.Sticky);
            try
            {
                var sw = Stopwatch.StartNew();

                ServiceList serviceListSource = BootstrapEditorSettingsUtility.GetValue(
                    asset => asset.EditModeServices,
                    out SettingsScope definedScope);
                if (serviceListSource.IsNull())
                {
                    sw.Stop();
                    Progress.Report(
                        progressId,
                        1f,
                        $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms. No edit mode services defined in {definedScope} scope.");
                    Progress.Finish(progressId);
                    return;
                }

                _editModeServiceListInstance = serviceListSource.CreateClone();
                SessionState.SetString(EditModeServicesEntityIdSessionStateKey, _editModeServiceListInstance.GetEntityId().ToString());

                var context = new BootstrapContext(TaskScheduler);
                var description = $"Creating edit mode services from prefab '{serviceListSource.name}' from {definedScope} scope.";
                Progress.Report(progressId, 1f, description);
                EditorUtility.DisplayProgressBar("Edit Mode Bootstrapping", description, 1f);
                ServiceLocator.Create(context, _editModeServiceListInstance);

                Logging.Trace($"Waiting for idle {nameof(TaskScheduler)}.");
                while (!TaskScheduler.IsIdle)
                {
                    await TaskScheduler.FlushQueueAsync();
                }

                EditorUtility.ClearProgressBar();
                BootstrapState = AppBootstrapState.Ready;

                sw.Stop();
                Progress.Report(
                    progressId,
                    1f,
                    $"Finished in {sw.Elapsed.TotalMilliseconds:0.00}ms with instance '{_editModeServiceListInstance.name}' from {definedScope} scope.");
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

        private void OnEditModeServicesChanged(ServiceList serviceList)
        {
            ReinitializeIfNecessary();
        }
    }
}
#endif