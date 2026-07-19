using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Custom inspector for <see cref="ServiceList"/> that warns about and allows clearing managed references
    /// with missing types.
    /// </summary>
    [CustomEditor(typeof(ServiceList))]
    public class ServiceListAssetEditor : UnityEditor.Editor
    {
        private const string HelpBoxTxt = "There are SerializeReference types which are missing.";
        private VisualElement _rootVisualElement;

        /// <inheritdoc />
        public override VisualElement CreateInspectorGUI()
        {
            _rootVisualElement = new VisualElement();
            RecreateUI();
            return _rootVisualElement;
        }

        private void RecreateUI()
        {
            _rootVisualElement.Clear();
            bool hasManagedReferencesWithMissingTypes = targets.Any(SerializationUtility.HasManagedReferencesWithMissingTypes);
            if (hasManagedReferencesWithMissingTypes)
            {
                _rootVisualElement.Add(new HelpBox(HelpBoxTxt, HelpBoxMessageType.Warning));
                _rootVisualElement.Add(
                    new Button(OnClickRemoveMissingManagedReferencesButton)
                    {
                        text = "Remove Missing Managed References",
                    });
            }

            InspectorElement.FillDefaultInspector(_rootVisualElement, serializedObject, this);
            _rootVisualElement.Bind(serializedObject);
        }

        private void OnClickRemoveMissingManagedReferencesButton()
        {
            foreach (Object t in targets)
            {
                SerializationUtility.ClearAllManagedReferencesWithMissingTypes(t);
                EditorUtility.SetDirty(t);
                AssetDatabase.SaveAssetIfDirty(t);
            }

            RecreateUI();
        }
    }
}