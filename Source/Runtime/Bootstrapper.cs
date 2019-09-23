using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace ASF.Core.Runtime
{
    public abstract class Bootstrapper : MonoBehaviour
    {
        #region Types

        public enum BootstrapperState
        {
            WaitingOnBootstrap,
            Complete
        }

        #endregion

        #region Fields

        public const string LOADED_SCENES_KEY = "BOOTSTRAPPER_LOADED_SCENES";

        public const string CONFIGURATION_KEY = "BOOTSTRAPPER_CONFIGURATION";

        protected static Bootstrapper Instance;

        #endregion

        #region Properties

        public BootstrapperState State { get; private set; } = BootstrapperState.Complete;

        #endregion

        #region Methods

        private static void RestoreDesiredScenes()
        {
#if UNITY_EDITOR
            var desiredScenesStr = UnityEditor.SessionState.GetString(LOADED_SCENES_KEY, null);
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
                    UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(
                        path,
                        new LoadSceneParameters(i == 0 
                            ? LoadSceneMode.Single 
                            : LoadSceneMode.Additive));
                }
            }
#else
            SceneManager.LoadScene(1);
#endif
        }

        public abstract void SetConfigurationFromJson(string json);

        public abstract string GetConfigurationAsJson();

        protected abstract UniTask BootstrapAppAsync();

        public async UniTask StartBootstrapAsync()
        {
            if (State != BootstrapperState.Complete)
            {
                return;
            }

            State = BootstrapperState.WaitingOnBootstrap;
#if UNITY_EDITOR
            var jsonConfig = UnityEditor.SessionState.GetString(CONFIGURATION_KEY, null);
            if (!string.IsNullOrWhiteSpace(jsonConfig))
            {
                SetConfigurationFromJson(jsonConfig);
            }
#endif
            await BootstrapAppAsync();

            State = BootstrapperState.Complete;
            RestoreDesiredScenes();
        }

        protected virtual void Awake()
        {
            EnsureSingleInstance();
        }

        protected virtual async void Start()
        {
            if (EnsureSingleInstance())
            {
                await StartBootstrapAsync();
            }
        }

        protected virtual void OnValidate()
        {
            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetAsFirstSibling();
            name = "--BOOTSTRAP--";
        }

        protected virtual void Reset()
        {
            OnValidate();
        }

        protected bool ShouldDestroySelf()
        {
            return Instance != null && Instance != this;
        }

        private bool EnsureSingleInstance()
        {
            if (ShouldDestroySelf())
            {
                Destroy(gameObject);
                return false;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            return true;
        }

        #endregion
    }
}