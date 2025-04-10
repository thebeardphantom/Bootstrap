﻿using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public interface IBootstrapEditorSettingsAsset
    {
        SettingsProperty<bool> EditorFlowEnabled { get; set; }

        SettingsProperty<EditModeBootstrapEnvironmentAsset> EditModeEnvironment { get; }

        SettingsProperty<RuntimeBootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; }

        SettingsProperty<RuntimeBootstrapEnvironmentAsset> DefaultBuildEnvironment { get; }

        SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; }

        SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; }

        SettingsProperty<MappedEnvironmentCollection<BuildProfile>> BuildProfileEnvironments { get; }
        
        SettingsProperty<BootstrapLogLevel> MinLogLevel { get; }

        void Save();
    }
}