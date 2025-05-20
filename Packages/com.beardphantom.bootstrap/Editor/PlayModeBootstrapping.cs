using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class PlayModeBootstrapping
    {
        static PlayModeBootstrapping()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange mode)
        {
            switch (mode)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    PrepareForEnteringPlayMode();
                    break;
                }
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
                    {
                        EditorSceneManager.playModeStartScene = default;
                    }

                    App.DeinitializeIfInMode(AppInitMode.PlayMode);
                    EditorApplication.delayCall += () => EditModeBootstrapping.PerformBootstrappingAsync().Forget();
                    break;
                }
            }
        }

        private static void PrepareForEnteringPlayMode()
        {
            EditModeBootstrapping.Cleanup();
            SessionState.EraseString(EditorBootstrapHandler.EditModeState);

            if (!BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
            {
                return;
            }

            if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Bootstrapper does not support untitled scenes."))
            {
                EditorApplication.isPlaying = false;
                return;
            }

            if (!TryFindActiveSceneEnvironment(out BootstrapEnvironmentAsset environment))
            {
                environment = BootstrapEditorSettingsUtility.GetValue(a => a.DefaultPlayModeEnvironment);
            }

            if (environment == null)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = default;
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            Logging.Trace("BootstrapEditorHelper prepping for playmode.");
            EditorBuildSettingsScene bootstrapScene = EditorBuildSettings.scenes.FirstOrDefault(
                s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != default);
            if (bootstrapScene == default)
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
                    .Where(g => g != null && g.scene.IsValid())
                    .Select(SelectedObjectPath.CreateInstance)
                    .ToArray();
                var editModeState = new EditModeState
                {
                    Environment = environment,
                    LoadedScenes = scenePaths,
                    SelectedObjects = selectedObjectPaths,
                };
                string editModeStateJson = JsonConvert.SerializeObject(editModeState);
                SessionState.SetString(EditorBootstrapHandler.EditModeState, editModeStateJson);
            }
        }

        private static bool TryFindActiveSceneEnvironment(out BootstrapEnvironmentAsset environment)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            MappedEnvironmentCollection<SceneAsset> sceneEnvironments = BootstrapEditorSettingsUtility.GetValue(
                a => a.EditorSceneEnvironments);
            return sceneEnvironments.TryFindEnvironmentForKey(sceneAsset, out environment);
        }
    }
}