#if UNITY_EDITOR
using System;
using UnityEditor.SceneManagement;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

namespace ASF.Core.Runtime
{
    public class EditorBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        #region Fields

        public const string LOADED_SCENES_KEY = "BOOTSTRAPPER_LOADED_SCENES";

        public const string SERIALIZED_BOOTSTRAPPER_JSON_KEY = "BOOTSTRAPPER_CONFIGURATION";

        public const char LOADED_SCENES_SEPARATOR = '\n';

        public static readonly char[] LoadedScenesSeparators =
        {
            LOADED_SCENES_SEPARATOR
        };

        #endregion

        #region Methods

        public static void OverwriteFromSerializedObject(Bootstrapper bootstrapper)
        {
            var jsonConfig = SessionState.GetString(SERIALIZED_BOOTSTRAPPER_JSON_KEY, null);
            if (!string.IsNullOrWhiteSpace(jsonConfig))
            {
                EditorJsonUtility.FromJsonOverwrite(jsonConfig, bootstrapper);
            }
        }

        public static string[] GetEditModeScenePaths()
        {
            var editModeScenesStr = SessionState.GetString(LOADED_SCENES_KEY, null);
            if (string.IsNullOrWhiteSpace(editModeScenesStr))
            {
                return null;
            }

            var editModeScenePaths = editModeScenesStr.Split(
                LoadedScenesSeparators,
                StringSplitOptions.RemoveEmptyEntries);
            return editModeScenePaths.Length == 0 ? null : editModeScenePaths;
        }

        public static void LoadScenesInPlayMode(IReadOnlyList<string> scenePaths)
        {
            Assert.IsNotNull(scenePaths, "editModeScenePaths != null");
            Assert.IsTrue(scenePaths.Count > 0, "editModeScenePaths.Length > 0");

            var loadFirstAsSingle = SceneManager.sceneCount <= 1;
            for (var i = 0; i < scenePaths.Count; i++)
            {
                var path = scenePaths[i];
                var loadSceneParameters = new LoadSceneParameters(
                    loadFirstAsSingle && i == 0
                        ? LoadSceneMode.Single
                        : LoadSceneMode.Additive);
                EditorSceneManager.LoadSceneInPlayMode(path, loadSceneParameters);
            }
        }

        /// <inheritdoc />
        public void OnPreBootstrap(Bootstrapper bootstrapper)
        {
            OverwriteFromSerializedObject(bootstrapper);
        }

        /// <inheritdoc />
        public void OnPostBootstrap(Bootstrapper bootstrapper)
        {
            var editModeScenePaths = GetEditModeScenePaths();
            if (editModeScenePaths != null)
            {
                LoadScenesInPlayMode(editModeScenePaths);
            }
        }

        #endregion
    }
}

#endif