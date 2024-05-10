#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.Addressables
{
    public partial class AddressablesServicesPrefabLoader
    {
        #region Methods

        protected override void SetServicesPrefab(GameObject servicesPrefab)
        {
            var success = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(servicesPrefab, out var guid, out var id);
            Assert.IsTrue(success, "success");
            PrefabReference = new AssetReferenceT<GameObject>(guid);
        }

        #endregion
    }
}
#endif