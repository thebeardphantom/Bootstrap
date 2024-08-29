using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public class DirectPrefabProvider : PrefabProvider
    {
        [field: SerializeField]
        private GameObject Prefab { get; set; }

        /// <inheritdoc />
        public override Awaitable<GameObject> LoadPrefabAsync()
        {
            return AwaitableUtility.FromResult(Prefab);
        }

        /// <inheritdoc />
        protected override void SetPrefab(GameObject prefab)
        {
            Prefab = prefab;
        }
    }
}