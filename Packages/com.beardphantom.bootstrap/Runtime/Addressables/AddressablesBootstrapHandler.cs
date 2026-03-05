#if UNITY_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Addressables
{
    [Serializable]
    public class AddressablesBootstrapHandler : IPostBootstrapHandler
    {
        [field: SerializeField]
        private string Key { get; set; }

        [field: SerializeField]
        private LoadSceneParameters LoadSceneParameters { get; set; }

        /// <inheritdoc />
        public async Awaitable OnPostBootstrapAsync(BootstrapContext context)
        {
            await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(
                    Key,
                    LoadSceneParameters,
                    SceneReleaseMode.ReleaseSceneWhenSceneUnloaded)
                .Task;
        }
    }
}
#endif