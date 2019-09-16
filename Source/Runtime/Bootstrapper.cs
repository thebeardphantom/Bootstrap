using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

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

        #endregion

        #region Properties

        public static Bootstrapper Instance { get; private set; }

        public BootstrapperState State { get; private set; } = BootstrapperState.Complete;

        #endregion

        #region Methods

        private static void RestoreDesiredScenes()
        {
#if UNITY_EDITOR
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
                        new LoadSceneParameters(i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive));
                }
            }
#else
            SceneManager.LoadScene(1);
#endif
        }

        public async void BeginBootstrapping()
        {
            if (State != BootstrapperState.Complete)
            {
                return;
            }

            State = BootstrapperState.WaitingOnBootstrap;
            await GetBootstrapTasks();
            State = BootstrapperState.Complete;
            RestoreDesiredScenes();
        }

        protected virtual async Task GetBootstrapTasks()
        {
            await Task.CompletedTask;
        }

        protected virtual void Awake()
        {
            EnsureSingleInstance();
        }

        protected virtual void Start()
        {
            if (EnsureSingleInstance())
            {
                BeginBootstrapping();
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