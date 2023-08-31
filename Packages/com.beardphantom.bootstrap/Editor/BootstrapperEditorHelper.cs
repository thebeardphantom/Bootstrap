using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class BootstrapperEditorHelper
    {
        #region Properties

        public static bool EnableSceneManagement { get; set; } = true;

        #endregion

        #region Constructors

        static BootstrapperEditorHelper()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        #endregion

        #region Methods

        private static void OnPlayModeStateChanged(PlayModeStateChange mode)
        {
            switch (mode)
            {
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (EnableSceneManagement)
                    {
                        PrepareForEnteringPlayMode();
                    }

                    break;
                }
                case PlayModeStateChange.EnteredEditMode:
                {
                    App.CleanupEditorOnly();
                    break;
                }
            }
        }

        private static void PrepareForEnteringPlayMode()
        {
            SessionState.EraseString(EditorBootstrapHandler.EDIT_MODE_STATE);
            EditorSceneManager.playModeStartScene = default;

            var bootstrapper = Object.FindObjectOfType<Bootstrapper>();
            if (bootstrapper == null || !bootstrapper.isActiveAndEnabled)
            {
                return;
            }

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
                        if (scene.buildIndex != 0)
                        {
                            scenePaths.Add(scene.path);
                        }
                    }

                    var bootstrapperJson = EditorJsonUtility.ToJson(bootstrapper);
                    var selectedObjectPaths = Selection.gameObjects
                        .Where(g => g != null && g.scene.IsValid())
                        .Select(SelectedObjectPath.CreateInstance)
                        .ToArray();
                    var editModeState = new EditModeState
                    {
                        BootstrapperJson = bootstrapperJson,
                        LoadedScenes = scenePaths,
                        SelectedObjects = selectedObjectPaths
                    };
                    var editModeStateJson = EditorJsonUtility.ToJson(editModeState);
                    SessionState.SetString(EditorBootstrapHandler.EDIT_MODE_STATE, editModeStateJson);
                }
            }
        }

        #endregion
    }
}