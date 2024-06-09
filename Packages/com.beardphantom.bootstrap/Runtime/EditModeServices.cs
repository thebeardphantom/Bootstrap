#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [DisallowMultipleComponent]
    public class EditModeServices : MonoBehaviour
    {
        #region Properties

        public EditModeServices SourceComponent { get; internal set; }
        
        public GameObject SourcePrefab { get; internal set; }

        #endregion
    }
}
#endif