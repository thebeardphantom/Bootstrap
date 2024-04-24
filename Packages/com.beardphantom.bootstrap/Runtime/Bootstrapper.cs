using BeardPhantom.Bootstrap.Logging;
using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        internal GameObject _servicesPrefab;

        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        #endregion

        #region Methods

        private void AssignBootstrapHandlers()
        {
            var foundPreHandler = TryGetComponent(out _preHandler);
            var foundPostHandler = TryGetComponent(out _postHandler);

#if UNITY_EDITOR
            var editorBootstrapHandler = new EditorBootstrapHandler();
            _preHandler = foundPreHandler ? _preHandler : editorBootstrapHandler;
            _postHandler = foundPostHandler ? _postHandler : editorBootstrapHandler;
#else
            var bootstrapHandler = new BuildBootstrapHandler();
            _preHandler = foundPreHandler ? _preHandler : bootstrapHandler;
            _postHandler = foundPostHandler ? _postHandler : bootstrapHandler;
#endif

            Log.Verbose($"Selected IPreBootstrapHandler {_preHandler}.", this);
            Log.Verbose($"Selected IPostBootstrapHandler {_postHandler}.", this);
        }

        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private async UniTaskVoid Start()
        {
            if (gameObject.scene.buildIndex == 0)
            {
                App.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
                Log.Info("Bootstrapping application.", this);
                AssignBootstrapHandlers();

                App.BootstrapState = AppBootstrapState.PreBootstrap;
                Log.Verbose("Beginning pre-bootstrapping.", this);
                await _preHandler.OnPreBootstrapAsync(this);

                App.BootstrapState = AppBootstrapState.ServiceCreation;
                Log.Verbose("Creating services.", this);
                await App.ServiceLocator.CreateAsync(_servicesPrefab);

                App.BootstrapState = AppBootstrapState.PostBoostrap;
                Log.Verbose("Beginning post-boostrapping.", this);
                await _postHandler.OnPostBootstrap(this);

                App.BootstrapState = AppBootstrapState.Ready;
                Log.Info("Bootstrapping complete.", this);
            }
            else
            {
                Log.Verbose("Destroying Bootstrapper from non-zero indexed scene.", this);
                Destroy(gameObject);
            }
        }

        #endregion
    }
}