using System.Collections.Generic;
using RSG;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEngine;

#endif

namespace ASFUnity.Core.Runtime
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

        private const string BOOTSTRAPPER_LOADED_SCENES = "BOOTSTRAPPER_LOADED_SCENES";

        [Min(0)]
        [SerializeField]
        private int _bootstrapSceneBuildIndex;

        #endregion

        #region Properties

        public static Bootstrapper Instance { get; private set; }

        public IPromise BootstrapPromise { get; private set; }

        public BootstrapperState State { get; private set; } = BootstrapperState.Complete;

        #endregion

        #region Methods

        protected abstract IPromise GetBootstrapTasks();

        public void BeginBootstrapping()
        {
            if (State != BootstrapperState.Complete)
            {
                return;
            }

            State = BootstrapperState.WaitingOnBootstrap;
            BootstrapPromise = GetBootstrapTasks();
            BootstrapPromise.Done(
                () =>
                {
                    State = BootstrapperState.Complete;
                    RestoreDesiredScenes();
                });
        }

        protected virtual void Awake()
        {
            if (EnsureInstance())
            {
                StoreDesiredScenes();
                SceneManager.LoadScene(_bootstrapSceneBuildIndex);
            }
        }

        protected virtual void Start()
        {
            if (EnsureInstance())
            {
                BeginBootstrapping();
            }
        }

        protected void OnValidate()
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

        private bool EnsureInstance()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return false;
            }

            Instance = this;
            DontDestroyOnLoad(this);
            return true;
        }

        private void StoreDesiredScenes()
        {
#if UNITY_EDITOR
            var scenePaths = new List<string>();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.buildIndex != _bootstrapSceneBuildIndex)
                {
                    scenePaths.Add(scene.path);
                }
            }

            var scenesString = string.Join("\n", scenePaths);
            SessionState.SetString(BOOTSTRAPPER_LOADED_SCENES, scenesString);
#endif
        }

        private void RestoreDesiredScenes()
        {
#if UNITY_EDITOR
            var desiredScenesStr = SessionState.GetString(BOOTSTRAPPER_LOADED_SCENES, null);
            if (desiredScenesStr == null)
            {
                SceneManager.LoadScene(_bootstrapSceneBuildIndex + 1);
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
            SceneManager.LoadScene(_bootstrapSceneBuildIndex + 1);
#endif
        }

        #endregion
    }
}