#if UNITY_EDITOR
using System;
using UnityEditor;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Helper methods for resolving <see cref="SettingsProperty{T}"/> values across the user and project settings
    /// scopes.
    /// </summary>
    public static class BootstrapEditorSettingsUtility
    {
        /// <summary>
        /// Resolves a settings value, preferring the user-scoped override when enabled and falling back to the
        /// project-scoped value otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the settings value.</typeparam>
        /// <param name="valueSelector">Selects the <see cref="SettingsProperty{T}"/> to read from a settings asset.</param>
        public static T GetValue<T>(Func<IBootstrapEditorSettingsAsset, SettingsProperty<T>> valueSelector)
        {
            return GetValue(valueSelector, out _);
        }

        /// <summary>
        /// Resolves a settings value, preferring the user-scoped override when enabled and falling back to the
        /// project-scoped value otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the settings value.</typeparam>
        /// <param name="valueSelector">Selects the <see cref="SettingsProperty{T}"/> to read from a settings asset.</param>
        /// <param name="definedScope">The scope the returned value was resolved from.</param>
        public static T GetValue<T>(
            Func<IBootstrapEditorSettingsAsset, SettingsProperty<T>> valueSelector,
            out SettingsScope definedScope)
        {
            SettingsProperty<T> property = valueSelector(BootstrapEditorUserSettings.instance);
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
        /// <param name="scope">The settings scope to get the asset for.</param>
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