using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

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

            EditorSceneManager.playModeStartScene = default;
            Bootstrapper bootstrapper = FindActiveSceneBootstrapper();
            if (bootstrapper == null || !bootstrapper.isActiveAndEnabled)
            {
                return;
            }

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
            }
            else
            {
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

                    if (bootstrapper.gameObject.scene.buildIndex != 0)
                    {
                        Log.Info(
                            $"BootstrapEditorHelper saving custom bootstrapper from scene '{bootstrapper.gameObject.scene.path}' to path '{EditorBootstrapHandler.TempBootstrapperPath}'.");
                        GameObject bootstrapperClone = Object.Instantiate(bootstrapper.gameObject);
                        PrefabUtility.SaveAsPrefabAsset(bootstrapperClone, EditorBootstrapHandler.TempBootstrapperPath);
                        Object.DestroyImmediate(bootstrapperClone);
                    }

                    SelectedObjectPath[] selectedObjectPaths = Selection.gameObjects
                        .Where(g => g != null && g.scene.IsValid())
                        .Select(SelectedObjectPath.CreateInstance)
                        .ToArray();
                    var editModeState = new EditModeState
                    {
                        LoadedScenes = scenePaths,
                        SelectedObjects = selectedObjectPaths,
                    };
                    string editModeStateJson = JsonConvert.SerializeObject(editModeState);
                    SessionState.SetString(EditorBootstrapHandler.EditModeState, editModeStateJson);
                }
            }
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