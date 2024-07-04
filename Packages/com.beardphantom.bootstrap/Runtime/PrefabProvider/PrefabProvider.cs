using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [DisallowMultipleComponent]
    public abstract class PrefabProvider : MonoBehaviour
    {
        internal static T Create<T>(GameObject host, GameObject prefab) where T : PrefabProvider
        {
            if (!host.TryGetComponent(out T cmp))
            {
                cmp = host.AddComponent<T>();
            }

            cmp.SetPrefab(prefab);
            return cmp;
        }

        public abstract UniTask<GameObject> LoadPrefabAsync();

        protected abstract void SetPrefab(GameObject prefab);
    }
}