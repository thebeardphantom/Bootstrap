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

            if (prefab)
            {
                cmp.SetPrefab(prefab);
            }

            return cmp;
        }

        public abstract Awaitable<GameObject> LoadPrefabAsync();

        protected abstract void SetPrefab(GameObject prefab);
    }
}