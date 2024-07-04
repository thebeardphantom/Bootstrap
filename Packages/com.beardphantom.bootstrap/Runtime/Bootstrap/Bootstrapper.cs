using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class Bootstrapper : MonoBehaviour
    {
        private IPreBootstrapHandler _preHandler;

        private IPostBootstrapHandler _postHandler;

        private bool _isOverrideInstance;

        [field: SerializeField]
        internal PrefabProvider PrefabProvider { get; set; }

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

            var context = new BootstrapContext(this);
            Assert.IsNotNull(PrefabProvider, "ServicesPrefabLoader != null");

            App.BootstrapState = AppBootstrapState.BootstrapHandlerDiscovery;
            Log.Info("Bootstrapping application.", this);
            AssignBootstrapHandlers();

            App.BootstrapState = AppBootstrapState.PreBootstrap;
            Log.Verbose("Beginning pre-bootstrapping.", this);
            await _preHandler.OnPreBootstrapAsync(context);

            App.BootstrapState = AppBootstrapState.ServicePrefabLoad;
            Log.Verbose($"Loading services prefab via loader {PrefabProvider}.", this);
            var servicesPrefab = await PrefabProvider.LoadPrefabAsync();

            App.BootstrapState = AppBootstrapState.ServiceCreation;
            Log.Verbose("Creating services.", this);
            servicesPrefab.SetActive(false);
            var servicesInstance = Instantiate(servicesPrefab);
            DontDestroyOnLoad(servicesInstance);
            servicesInstance.name = servicesPrefab.name;
            servicesPrefab.SetActive(true);
            ClearDirtyFlag(servicesPrefab);
            await App.ServiceLocator.CreateAsync(context, servicesInstance);

            App.BootstrapState = AppBootstrapState.PostBoostrap;
            Log.Verbose("Beginning post-boostrapping.", this);
            await _postHandler.OnPostBootstrapAsync(context, this);

            App.BootstrapState = AppBootstrapState.Ready;
            Log.Info("Bootstrapping complete.", this);
        }

        partial void ClearDirtyFlag(GameObject servicesPrefab);

        partial void TryReplaceWithOverrideInstance();
    }
}