#if UNITY_ADDRESSABLES
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BeardPhantom.Bootstrap.Addressables
{
    public partial class AddressablesPrefabProvider : PrefabProvider
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

        protected override void SetPrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            SetServicesPrefabInEditor(prefab);
#else
            throw new System.Exception("Cannot set prefab outside of editor.");
#endif
        }

        #endregion
    }
}
#endif