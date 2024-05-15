using BeardPhantom.Bootstrap.Logging;
using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper : MonoBehaviour
    {
        #region Fields

        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        private bool _isOverrideInstance;

        #endregion

        #region Properties

        [field: SerializeField]
        internal PrefabProvider PrefabProvider { get; set; }

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

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "Unity.IncorrectMethodSignature")]
        private async UniTaskVoid Start()
        {
            Assert.IsTrue(gameObject.scene.buildIndex == 0, "gameObject.scene.buildIndex == 0");

#if UNITY_EDITOR
            if (!_isOverrideInstance)
            {
                TryReplaceWithOverrideInstance();
                if (this == null)
                {
                    // this instance was destroyed
                    return;
                }
            }
#endif

            Assert.IsNotNull(PrefabProvider, "ServicesPrefabLoader != null");

            App.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
            Log.Info("Bootstrapping application.", this);
            AssignBootstrapHandlers();

            App.BootstrapState = AppBootstrapState.PreBootstrap;
            Log.Verbose("Beginning pre-bootstrapping.", this);
            await _preHandler.OnPreBootstrapAsync(this);

            App.BootstrapState = AppBootstrapState.ServicePrefabLoad;
            Log.Verbose($"Loading services prefab via loader {PrefabProvider}.", this);
            var servicesPrefab = await PrefabProvider.LoadPrefabAsync();

            App.BootstrapState = AppBootstrapState.ServiceCreation;
            Log.Verbose("Creating services.", this);
            await App.ServiceLocator.CreateAsync(servicesPrefab);

            App.BootstrapState = AppBootstrapState.PostBoostrap;
            Log.Verbose("Beginning post-boostrapping.", this);
            await _postHandler.OnPostBootstrapAsync(this);

            App.BootstrapState = AppBootstrapState.Ready;
            Log.Info("Bootstrapping complete.", this);
        }

        partial void TryReplaceWithOverrideInstance();

        #endregion
    }
}