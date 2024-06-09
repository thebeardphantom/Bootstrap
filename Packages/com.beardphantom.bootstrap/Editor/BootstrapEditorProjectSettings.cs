using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    [FilePath("ProjectSettings/BootstrapProjectSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class BootstrapEditorProjectSettings : BootstrapEditorSettingsAsset<BootstrapEditorProjectSettings>,
        ISerializationCallbackReceiver
    {
        #region Methods

        /// <inheritdoc />
        public void OnBeforeSerialize() { }

        /// <inheritdoc />
        public void OnAfterDeserialize()
        {
            base.EditorFlowEnabled.OverrideEnabled = true;
            base.EditModeServices.OverrideEnabled = true;
        }

        #endregion
    }
}