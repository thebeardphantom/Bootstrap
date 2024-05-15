#if UNITY_EDITOR && UNITY_ADDRESSABLES
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.Addressables
{
    public partial class AddressablesPrefabProvider
    {
        #region Methods

        private void SetServicesPrefabInEditor(GameObject prefab)
        {
            var success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out var guid, out long id);
            Assert.IsTrue(success, "success");
            PrefabReference = new AssetReferenceT<GameObject>(guid);
        }

        #endregion
    }
}
#endif