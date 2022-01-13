using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BeardPhantom.Fabric.Core
{
    public sealed class Bootstrapper : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private GameObject _servicesPrefab;

        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        #endregion

        #region Methods

        private void Reset()
        {
            ResetGameObjectInHierarchy();
        }

        private void OnValidate()
        {
            ResetGameObjectInHierarchy();
        }

        private void ResetGameObjectInHierarchy()
        {
            transform.parent = null;
            transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            transform.SetAsFirstSibling();
            name = "--BOOTSTRAP--";
        }

        private void AssignBootstrapHandlers()
        {
#if UNITY_EDITOR
            var editorBootstrapHandler = new EditorBootstrapHandler();
            _preHandler = editorBootstrapHandler;
            _postHandler = editorBootstrapHandler;
#else
            var bootstrapHandler = new BuildBootstrapHandler();
            _preHandler = bootstrapHandler;
            _postHandler = bootstrapHandler;
#endif
        }

        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private async UniTaskVoid Start()
        {
            if (gameObject.scene.buildIndex != 0)
            {
                Destroy(gameObject);
            }
            else
            {
                AssignBootstrapHandlers();
                _preHandler.OnPreBootstrap(this);
                await App.Instance.ServiceLocator.CreateAsync(_servicesPrefab);
                _postHandler.OnPostBootstrap(this);
            }
        }

        #endregion
    }
}