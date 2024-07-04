using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public class DirectPrefabProvider : PrefabProvider
    {
        [field: SerializeField]
        private GameObject Prefab { get; set; }

        /// <inheritdoc />
        public override UniTask<GameObject> LoadPrefabAsync()
        {
            return UniTask.FromResult(Prefab);
        }

        /// <inheritdoc />
        protected override void SetPrefab(GameObject prefab)
        {
            Prefab = prefab;
        }
    }
}