#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [DisallowMultipleComponent]
    public class EditModeServices : MonoBehaviour
    {
        public EditModeServices SourceComponent { get; internal set; }

        public GameObject SourcePrefab { get; internal set; }
    }
}
#endif