// #undef UNITY_EDITOR

#if UNITY_ADDRESSABLES

using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEngine.Assertions;
using UnityEditor;
#else
using System;
#endif

namespace BeardPhantom.Bootstrap.Addressables
{
    public partial class AddressablesPrefabProvider : PrefabProvider
    {
        [field: SerializeField]
        private AssetReferenceT<GameObject> PrefabReference { get; set; }

        /// <inheritdoc />
        public override async Awaitable<GameObject> LoadPrefabAsync()
        {
            return await UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(PrefabReference).Task;
        }

        protected override void SetPrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            SetServicesPrefabInEditor(prefab);
#else
            throw new Exception("Cannot set prefab outside of editor.");
#endif
        }
    }

#if UNITY_EDITOR
    public partial class AddressablesPrefabProvider
    {
        private void SetServicesPrefabInEditor(GameObject prefab)
        {
            bool success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long id);
            Assert.IsTrue(success, "success");
            PrefabReference = new AssetReferenceT<GameObject>(guid);
        }
    }
#endif
}
#endif
