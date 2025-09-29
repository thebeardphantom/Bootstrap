#if UNITY_ADDRESSABLES
using UnityEngine;

namespace BeardPhantom.Bootstrap.Addressables
{
    public class AddressablesBootstrapHandler : MonoBehaviour, IPostBootstrapHandler
    {
        [field: SerializeField]
        private string Key { get; set; }

        /// <inheritdoc />
        public async Awaitable OnPostBootstrapAsync(BootstrapContext context)
        {
            await UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(Key).Task;
        }
    }
}
#endif
