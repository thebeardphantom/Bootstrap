using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    [CustomEditor(typeof(SceneAsset))]
    public class SceneAssetEditor : UnityEditor.Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            root.Add(
                new ObjectField("Bootstrap Environment")
                {
                    objectType = typeof(RuntimeBootstrapEnvironmentAsset),
                    allowSceneObjects = false,
                });
            return root;
        }
    }
}