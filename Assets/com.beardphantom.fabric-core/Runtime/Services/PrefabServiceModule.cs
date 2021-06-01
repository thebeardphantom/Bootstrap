using UnityEngine;
using UnityEngine.Assertions;

namespace Fabric.Core.Runtime
{
    public class PrefabServiceModule : ServiceModule
    {
        #region Fields

        private readonly GameObject _prefab;

        #endregion

        #region Constructors

        public PrefabServiceModule(GameObject prefab)
        {
            Assert.IsNotNull(_prefab);
            _prefab = prefab;
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        protected internal override void BindServices()
        {
            _prefab.SetActive(false);

            var instance = Object.Instantiate(_prefab);
            Object.DontDestroyOnLoad(instance);

            var monoBehaviours = instance.GetComponentsInChildren<MonoBehaviour>(true);
            foreach (var monoBehaviour in monoBehaviours)
            {
                Bind(monoBehaviour.GetType(), monoBehaviour);
            }
        }

        #endregion
    }
}