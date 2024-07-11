using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [FilePath("ProjectSettings/BootstrapProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorProjectSettings : BootstrapEditorSettingsAsset<BootstrapEditorProjectSettings>, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public bool VerboseLogging { get; private set; }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            EditorFlowEnabled.OverrideEnabled = true;
            EditModeServices.OverrideEnabled = true;
        }
    }
}