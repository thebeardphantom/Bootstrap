#if UNITY_ADDRESSABLES
using System;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Addressables
{
    /// <summary>
    /// Post-bootstrap handler that loads a scene through the Addressables system using the configured key.
    /// </summary>
    [Serializable]
    public class AddressablesBootstrapHandler : IPostBootstrapHandler
    {
        /// <summary>
        /// The Addressables key of the scene to load.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The Addressables key of the scene to load.")]
        private string Key { get; set; }

        /// <summary>
        /// The parameters used when loading the scene.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The parameters used when loading the scene.")]
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