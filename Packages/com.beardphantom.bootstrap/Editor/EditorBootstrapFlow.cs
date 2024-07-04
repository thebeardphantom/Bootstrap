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
        public static bool EnableSceneManagement { get; set; } = true;

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
                    if (EnableSceneManagement)
                    {
                        EditorSceneManager.playModeStartScene = default;
                    }

                    App.CleanupEditorOnly();
                    EditorApplication.delayCall += () => EditModeBootstrapping.PerformBootstrapping().Forget();
                    break;
                }
            }
        }

        private static void PrepareForEnteringPlayMode()
        {
            SessionState.EraseString(EditorBootstrapHandler.EditModeState);
            File.Delete(EditorBootstrapHandler.TempBootstrapperPath);

            if (!EnableSceneManagement)
            {
                return;
            }

            EditorSceneManager.playModeStartScene = default;
            var bootstrapper = FindActiveSceneBootstrapper();
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
            var bootstrapScene = EditorBuildSettings.scenes.FirstOrDefault(
                s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != default);
            if (bootstrapScene == default)
            {
                Log.Info("No valid first scene in EditorBuildSettings");
            }
            else
            {
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootstrapScene.path);
                EditorSceneManager.playModeStartScene = sceneAsset;

                using (ListPool<string>.Get(out var scenePaths))
                {
                    for (var i = 0; i < SceneManager.sceneCount; i++)
                    {
                        var scene = SceneManager.GetSceneAt(i);
                        if (scene.buildIndex != 0 && scene.IsValid())
                        {
                            scenePaths.Add(scene.path);
                        }
                    }

                    if (bootstrapper.gameObject.scene.buildIndex != 0)
                    {
                        Log.Info(
                            $"BootstrapEditorHelper saving custom bootstrapper from scene '{bootstrapper.gameObject.scene.path}' to path '{EditorBootstrapHandler.TempBootstrapperPath}'.");
                        var bootstrapperClone = Object.Instantiate(bootstrapper.gameObject);
                        PrefabUtility.SaveAsPrefabAsset(bootstrapperClone, EditorBootstrapHandler.TempBootstrapperPath);
                        Object.DestroyImmediate(bootstrapperClone);
                    }

                    var selectedObjectPaths = Selection.gameObjects
                        .Where(g => g != null && g.scene.IsValid())
                        .Select(SelectedObjectPath.CreateInstance)
                        .ToArray();
                    var editModeState = new EditModeState
                    {
                        LoadedScenes = scenePaths,
                        SelectedObjects = selectedObjectPaths,
                    };
                    var editModeStateJson = EditorJsonUtility.ToJson(editModeState);
                    SessionState.SetString(EditorBootstrapHandler.EditModeState, editModeStateJson);
                }
            }
        }

        private static Bootstrapper FindActiveSceneBootstrapper()
        {
            var bootstrappers = Object.FindObjectsByType<Bootstrapper>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
            var activeScene = SceneManager.GetActiveScene();
            return bootstrappers.FirstOrDefault(bootstrapper => bootstrapper.gameObject.scene.path == activeScene.path);
        }
    }
}