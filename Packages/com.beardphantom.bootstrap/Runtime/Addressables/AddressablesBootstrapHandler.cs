#if UNITY_ADDRESSABLES
using Cysharp.Threading.Tasks;
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
        public UniTask OnPostBootstrapAsync(Bootstrapper bootstrapper)
        {
            return UnityEngine.AddressableAssets.Addressables.LoadSceneAsync(Key).ToUniTask();
        }

        #endregion
    }
}
#endif