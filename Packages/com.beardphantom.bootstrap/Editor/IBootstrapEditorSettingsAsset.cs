namespace BeardPhantom.Bootstrap.Editor
{
    public interface IBootstrapEditorSettingsAsset
    {
        #region Properties

        SettingsProperty<bool> EditorFlowEnabled { get; set; }

        SettingsProperty<EditModeServices> EditModeServices { get; set; }

        #endregion

        #region Methods

        void Save();

        #endregion
    }
}