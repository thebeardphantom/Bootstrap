using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [FilePath("ProjectSettings/BootstrapProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorProjectSettings : BootstrapEditorSettingsAsset<BootstrapEditorProjectSettings>,
        ISerializationCallbackReceiver
    {
        /// <inheritdoc />
        public void OnBeforeSerialize() { }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            EditorFlowEnabled.OverrideEnabled = true;
            EditModeServices.OverrideEnabled = true;
        }
    }
}