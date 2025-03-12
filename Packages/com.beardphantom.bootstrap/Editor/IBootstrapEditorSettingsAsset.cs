﻿namespace BeardPhantom.Bootstrap.Editor
{
    public interface IBootstrapEditorSettingsAsset
    {
        SettingsProperty<bool> EditorFlowEnabled { get; set; }

        SettingsProperty<EditModeServices> EditModeServices { get; set; }
        
        SettingsProperty<LogLevel> MinLogLevel { get; set; }

        void Save();
    }
}