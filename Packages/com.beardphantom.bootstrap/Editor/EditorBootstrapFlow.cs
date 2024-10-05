using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class EditorBootstrapFlow
    {
        static EditorBootstrapFlow()
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
            File.Delete(EditorBootstrapHandler.TempBootstrapperPath);

            if (!BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
            {
                return;
            }

            if (!FindActiveSceneEnvironment(out RuntimeBootstrapEnvironmentAsset environment))
            {
                environment = BootstrapEditorSettingsUtility.GetValue(a => a.DefaultPlayModEnvironment);
                if (environment == null)
                {
                    return;
                }
            }

            EditorSceneManager.playModeStartScene = default;

            if (!EditorSceneManager.EnsureUntitledSceneHasBeenSaved("Bootstrapper does not support untitled scenes."))
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

            Log.Verbose("BootstrapEditorHelper prepping for playmode.");
            EditorBuildSettingsScene bootstrapScene = EditorBuildSettings.scenes.FirstOrDefault(
                s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != default);
            if (bootstrapScene == default)
            {
                Log.Info("No valid first scene in EditorBuildSettings");
                return;
            }

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootstrapScene.path);
            EditorSceneManager.playModeStartScene = sceneAsset;

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

        private static bool FindActiveSceneEnvironment(out RuntimeBootstrapEnvironmentAsset environment)
        {
            Scene activeScene = SceneManager.GetActiveScene();
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(activeScene.path);
            MappedEnvironmentCollection<SceneAsset> sceneEnvironments = BootstrapEditorSettingsUtility.GetValue(
                a => a.EditorSceneEnvironments);
            foreach (MappedEnvironment<SceneAsset> mappedEnvironment in sceneEnvironments)
            {
                if (mappedEnvironment.Key == sceneAsset)
                {
                    environment = mappedEnvironment.Environment;
                    return true;
                }
            }

            environment = default;
            return false;
        }

        private static Bootstrapper FindActiveSceneBootstrapper()
        {
            Bootstrapper[] bootstrappers = Object.FindObjectsByType<Bootstrapper>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
            Scene activeScene = SceneManager.GetActiveScene();
            return bootstrappers.FirstOrDefault(bootstrapper => bootstrapper.gameObject.scene.path == activeScene.path);
        }
    }
}