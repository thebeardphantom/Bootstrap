using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BeardPhantom.Bootstrap.Addressables
{
    public partial class AddressablesServicesPrefabLoader : ServicesPrefabLoader
    {
        #region Properties

        [field: SerializeField]
        private AssetReferenceT<GameObject> PrefabReference { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override UniTask<GameObject> LoadPrefabAsync()
        {
            return UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(PrefabReference).ToUniTask();
        }

        #endregion
    }
}