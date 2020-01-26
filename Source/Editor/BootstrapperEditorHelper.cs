using ASF.Core.Runtime;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ASF.Core.Editor
{
    [InitializeOnLoad]
    public static class BootstrapperEditorHelper
    {
        #region Fields

        public static bool EnableSceneManagement = true;

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
            if (!EnableSceneManagement || mode == PlayModeStateChange.EnteredPlayMode)
            {
                return;
            }

            SessionState.EraseString(EditorBootstrapHandler.LOADED_SCENES_KEY);
            SessionState.EraseString(EditorBootstrapHandler.CONFIGURATION_KEY);
            EditorSceneManager.playModeStartScene = null;

            var bootstrapper = Object.FindObjectOfType<Bootstrapper>();
            if (bootstrapper == null 
                || !bootstrapper.isActiveAndEnabled
                || mode != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }
            ASFCore.Logger.Log("ASF", "Running bootstrap helper...");

            var bootstrapScene = EditorBuildSettings.scenes
                .FirstOrDefault(s => AssetDatabase.LoadAssetAtPath<SceneAsset>(s.path) != null);
            if (bootstrapScene == null)
            {
                ASFCore.Logger.LogWarning("ASF", "No valid first scene in EditorBuildSettings");
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

                var scenesString = string.Join("\n", scenePaths);
                SessionState.SetString(EditorBootstrapHandler.LOADED_SCENES_KEY, scenesString);

                var configuration = EditorJsonUtility.ToJson(bootstrapper);
                if (!string.IsNullOrWhiteSpace(configuration))
                {
                    SessionState.SetString(EditorBootstrapHandler.CONFIGURATION_KEY, configuration);
                }
            }
        }

        #endregion
    }
}