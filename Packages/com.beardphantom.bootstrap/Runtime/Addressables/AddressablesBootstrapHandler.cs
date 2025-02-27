#if UNITY_ADDRESSABLES
using UnityEngine;

namespace BeardPhantom.Bootstrap.Addressables
{
    public class AddressablesBootstrapHandler : MonoBehaviour, IPostBootstrapHandler
    {
        #region Properties

        [field: SerializeField]
        private string Key { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        async Awaitable IPostBootstrapHandler.OnPostBootstrapAsync(BootstrapContext context, Bootstrapper bootstrapper)
        {
            await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(Key).Task;
        }

        #endregion
    }
}
#endif