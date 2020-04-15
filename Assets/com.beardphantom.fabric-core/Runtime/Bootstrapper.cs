using UniRx.Async;
using UnityEngine;

namespace Fabric.Core.Runtime
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

        protected static Bootstrapper Instance;

        protected IPreBootstrapHandler PreHandler;

        protected IPostBootstrapHandler PostHandler;

        #endregion

        #region Properties

        public BootstrapperState State { get; private set; } = BootstrapperState.Complete;

        #endregion

        #region Methods

        protected abstract UniTask BootstrapAppAsync();

        public async UniTask StartBootstrapAsync()
        {
            if (State != BootstrapperState.Complete)
            {
                return;
            }

            State = BootstrapperState.WaitingOnBootstrap;
            PreHandler.OnPreBootstrap(this);

            await BootstrapAppAsync();

            State = BootstrapperState.Complete;
            PostHandler.OnPostBootstrap(this);
        }

        protected virtual async void Awake()
        {
            if (!EnsureSingleInstance())
            {
                return;
            }

            AssignBootstrapHandlers(out PreHandler, out PostHandler);
            await StartBootstrapAsync();
        }

        protected virtual void AssignBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            var editorBootstrapHandler = new EditorBootstrapHandler();

            preHandler = editorBootstrapHandler;
            postHandler = editorBootstrapHandler;
#else
            var bootstrapHandler = new BuildBootstrapHandler();
            
            preHandler = bootstrapHandler;
            postHandler = bootstrapHandler;
#endif
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