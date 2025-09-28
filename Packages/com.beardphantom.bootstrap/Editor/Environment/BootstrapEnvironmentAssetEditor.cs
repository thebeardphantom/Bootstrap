using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [CustomEditor(typeof(BootstrapEnvironmentAsset))]
    public class BootstrapEnvironmentAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }
    }
}