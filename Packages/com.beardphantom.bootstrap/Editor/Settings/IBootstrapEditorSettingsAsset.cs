﻿using BeardPhantom.Bootstrap.Editor.Environment;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public interface IBootstrapEditorSettingsAsset
    {
        SettingsProperty<bool> EditorFlowEnabled { get; set; }

        SettingsProperty<GameObject> EditModeServices { get; }

        SettingsProperty<BootstrapEnvironmentAsset> DefaultPlayModeEnvironment { get; }

        SettingsProperty<BootstrapEnvironmentAsset> DefaultBuildEnvironment { get; }

        SettingsProperty<MappedEnvironmentCollection<string>> PlatformEnvironments { get; }

        SettingsProperty<MappedEnvironmentCollection<SceneAsset>> EditorSceneEnvironments { get; }

        SettingsProperty<MappedEnvironmentCollection<BuildProfile>> BuildProfileEnvironments { get; }
        
        SettingsProperty<BootstrapLogLevel> MinLogLevel { get; }

        void Save();
    }
}