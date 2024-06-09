using BeardPhantom.Bootstrap.Editor;
using System;
using UnityEditor;

public static class BootstrapEditorSettingsUtility
{
    #region Methods

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

    #endregion
}