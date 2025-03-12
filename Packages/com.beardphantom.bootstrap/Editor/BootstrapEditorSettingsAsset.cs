using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    public abstract class BootstrapEditorSettingsAsset<T> : ScriptableSingleton<T>, IBootstrapEditorSettingsAsset where T : ScriptableObject
    {
        [field: SerializeField]
        public SettingsProperty<bool> EditorFlowEnabled { get; set; } = new();

        [field: SerializeField]
        public SettingsProperty<EditModeServices> EditModeServices { get; set; } = new();
        
        [field: SerializeField]
        public SettingsProperty<LogLevel> MinLogLevel { get; set; } = new(LogLevel.Information);

        public void Save()
        {
            base.Save(true);
        }
    }
}