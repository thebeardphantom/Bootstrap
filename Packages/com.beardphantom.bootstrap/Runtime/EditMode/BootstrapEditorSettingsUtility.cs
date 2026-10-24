#if UNITY_EDITOR
using System;
using UnityEditor;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Helper methods for resolving <see cref="SettingsProperty{T}" /> values across the user and project settings
    /// scopes.
    /// </summary>
    public static class BootstrapEditorSettingsUtility
    {
        /// <summary>
        /// Resolves a settings value, preferring the user-scoped override when enabled and falling back to the
        /// project-scoped value otherwise.
        /// </summary>
        public static T GetValue<T>(Func<IBootstrapEditorSettingsAsset, SettingsProperty<T>> valueSelector)
        {
            return GetValue(valueSelector, out _);
        }

        /// <summary>
        /// Resolves a settings value, preferring the user-scoped override when enabled and falling back to the
        /// project-scoped value otherwise.
        /// </summary>
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

        /// <summary>
        /// Gets the settings asset for the given scope.
        /// </summary>
        public static IBootstrapEditorSettingsAsset GetWithScope(in SettingsScope scope)
        {
            return scope switch
            {
                SettingsScope.User => BootstrapEditorUserSettings.instance,
                SettingsScope.Project => BootstrapEditorProjectSettings.instance,
                _ => throw new ArgumentOutOfRangeException(nameof(scope), scope, null),
            };
        }
    }
}
#endif