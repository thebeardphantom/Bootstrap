#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public abstract partial class ServicesPrefabLoader
    {
        #region Methods

        public static T Create<T>(GameObject host, GameObject servicesPrefab) where T : ServicesPrefabLoader
        {
            var cmp = host.AddComponent<T>();
            cmp.SetServicesPrefab(servicesPrefab);
            return cmp;
        }

        protected abstract void SetServicesPrefab(GameObject servicesPrefab);

        #endregion
    }
}
#endif