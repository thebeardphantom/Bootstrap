using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Fabric.Core
{
    public abstract class Bootstrapper : MonoBehaviour
    {
        #region Fields

        protected IPreBootstrapHandler PreHandler;

        protected IPostBootstrapHandler PostHandler;

        #endregion

        #region Methods

        protected abstract UniTask BootstrapAppAsync();

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

        protected void Reset()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetAsFirstSibling();
            name = "--BOOTSTRAP--";
        }

        private async UniTaskVoid Start()
        {
            AssignBootstrapHandlers(out PreHandler, out PostHandler);
            PreHandler.OnPreBootstrap(this);
            await BootstrapAppAsync();
            PostHandler.OnPostBootstrap(this);
        }

        #endregion
    }
}