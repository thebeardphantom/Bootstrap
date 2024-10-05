using System;
using UnityEditor;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public static class BootstrapEditorSettingsUtility
    {
        public static T GetValue<T>(Func<IBootstrapEditorSettingsAsset, SettingsProperty<T>> valueSelector)
        {
            return GetValue(valueSelector, out var _);
        }

        public static T GetValue<T>(
            Func<IBootstrapEditorSettingsAsset, SettingsProperty<T>> valueSelector,
            out SettingsScope definedScope)
        {
            var property = valueSelector(BootstrapEditorUserSettings.instance);
            if (property.OverrideEnabled)
            {
                definedScope = SettingsScope.User;
                return property.Value;
            }

            definedScope = SettingsScope.Project;
            property = valueSelector(BootstrapEditorProjectSettings.instance);
            return property.Value;
        }

        public static IBootstrapEditorSettingsAsset GetWithScope(in SettingsScope scope)
        {
            return scope switch
            {
                SettingsScope.User => BootstrapEditorUserSettings.instance,
                SettingsScope.Project => BootstrapEditorProjectSettings.instance,
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null)
            };
        }
    }
}