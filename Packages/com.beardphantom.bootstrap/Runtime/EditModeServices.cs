#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [DisallowMultipleComponent]
    public class EditModeServices : MonoBehaviour
    {
        #region Properties

        public GameObject SourcePrefab { get; internal set; }

        #endregion
    }
}
#endif