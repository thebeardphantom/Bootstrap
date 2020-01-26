#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace ASF.Core.Runtime
{
    public class EditorBootstrapHandler : IPreBootstrapHandler, IPostBootstrapHandler
    {
        #region Fields

        public const string LOADED_SCENES_KEY = "BOOTSTRAPPER_LOADED_SCENES";

        public const string CONFIGURATION_KEY = "BOOTSTRAPPER_CONFIGURATION";

        #endregion

        #region Methods

        /// <inheritdoc />
        public void OnPreBootstrap(Bootstrapper bootstrapper)
        {
            var jsonConfig = SessionState.GetString(CONFIGURATION_KEY, null);
            if (!string.IsNullOrWhiteSpace(jsonConfig))
            {
                EditorJsonUtility.FromJsonOverwrite(jsonConfig, bootstrapper);
            }
        }

        /// <inheritdoc />
        public void OnPostBootstrap(Bootstrapper bootstrapper)
        {
            var desiredScenesStr = SessionState.GetString(LOADED_SCENES_KEY, null);
            if (string.IsNullOrWhiteSpace(desiredScenesStr))
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                var desiredScenePaths = desiredScenesStr.Split('\n');
                for (var i = 0; i < desiredScenePaths.Length; i++)
                {
                    var path = desiredScenePaths[i];
                    EditorSceneManager.LoadSceneInPlayMode(
                        path,
                        new LoadSceneParameters(
                            i == 0
                                ? LoadSceneMode.Single
                                : LoadSceneMode.Additive));
                }
            }
        }

        #endregion
    }
}

#endif