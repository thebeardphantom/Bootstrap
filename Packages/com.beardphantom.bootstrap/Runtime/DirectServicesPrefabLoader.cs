using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public partial class DirectServicesPrefabLoader : ServicesPrefabLoader
    {
        #region Properties

        [field: SerializeField]
        private GameObject ServicesPrefab { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override UniTask<GameObject> LoadPrefabAsync()
        {
            return UniTask.FromResult(ServicesPrefab);
        }

        #endregion
    }
}