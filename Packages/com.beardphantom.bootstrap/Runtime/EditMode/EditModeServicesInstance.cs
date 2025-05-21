#if UNITY_EDITOR
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    public class EditModeServicesInstance : MonoBehaviour
    {
        public GameObject SourcePrefab { get; internal set; }
    }
}
#endif