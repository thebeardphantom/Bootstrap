#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public partial class DirectServicesPrefabLoader
    {
        #region Methods

        /// <inheritdoc />
        protected override void SetServicesPrefab(GameObject servicesPrefab)
        {
            ServicesPrefab = servicesPrefab;
        }

        #endregion
    }
}
#endif