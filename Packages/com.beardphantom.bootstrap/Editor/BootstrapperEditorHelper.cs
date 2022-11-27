using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Editor
{
    [InitializeOnLoad]
    public static class BootstrapperEditorHelper
    {
        #region Fields

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
            EditorSceneManager.playModeStartScene = null;

            var bootstrapper = Object.FindObjectOfType<Bootstrapper>();
            if (bootstrapper == null || !bootstrapper.isActiveAndEnabled)
            {
                return;
            }

            Debug.Log("Running bootstrap helper...");

            var bootstrapScene = EditorBuildSettings.scenes.FirstOrDefault(
                s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != null);
            if (bootstrapScene == null)
            {
                Debug.Log("[Bootstrapper] No valid first scene in EditorBuildSettings");
            }
            else
            {
                var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(bootstrapScene.path);
                EditorSceneManager.playModeStartScene = sceneAsset;

                var scenePaths = new List<string>();
                for (var i = 0; i < SceneManager.sceneCount; i++)
                {
                    var scene = SceneManager.GetSceneAt(i);
                    if (scene.buildIndex != 0)
                    {
                        scenePaths.Add(scene.path);
                    }
                }

                var bootstrapperJson = EditorJsonUtility.ToJson(bootstrapper);
                if (!string.IsNullOrWhiteSpace(bootstrapperJson))
                {
                    var selectedObjectPaths = Selection.gameObjects
                        .Where(g => g != null && g.scene.IsValid())
                        .Select(SelectedObjectPath.CreateInstance)
                        .ToArray();
                    var editModeState = new EditModeState
                    {
                        BootstrapperJson = bootstrapperJson,
                        LoadedScenes = scenePaths.ToArray(),
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