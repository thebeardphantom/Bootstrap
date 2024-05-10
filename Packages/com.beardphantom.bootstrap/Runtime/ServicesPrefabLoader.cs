using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public abstract partial class ServicesPrefabLoader : MonoBehaviour
    {
        #region Methods

        public abstract UniTask<GameObject> LoadPrefabAsync();

        #endregion
    }
}