using System;
using UnityEditor;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor
{
    public abstract class BootstrapEditorSettingsAsset<T> : ScriptableSingleton<T>,
        IBootstrapEditorSettingsAsset
        where T : ScriptableObject
    {
        #region Properties

        [field: SerializeField]
        public SettingsProperty<bool> EditorFlowEnabled { get; set; } = new();

        [field: SerializeField]
        public SettingsProperty<EditModeServices> EditModeServices { get; set; } = new();

        #endregion

        #region Methods

        public void Save()
        {
            base.Save(true);
        }

        #endregion
    }
}