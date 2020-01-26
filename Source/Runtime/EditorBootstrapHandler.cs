#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using System;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

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

        public static void LoadScenesInPlayMode(string[] editModeScenePaths)
        {
            Assert.IsNotNull(editModeScenePaths, "editModeScenePaths != null");
            Assert.IsTrue(editModeScenePaths.Length > 0, "editModeScenePaths.Length > 0");

            for (var i = 0; i < editModeScenePaths.Length; i++)
            {
                var path = editModeScenePaths[i];
                EditorSceneManager.LoadSceneInPlayMode(
                    path,
                    new LoadSceneParameters(
                        i == 0
                            ? LoadSceneMode.Single
                            : LoadSceneMode.Additive));
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