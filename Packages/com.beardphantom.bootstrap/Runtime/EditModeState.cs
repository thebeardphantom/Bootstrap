using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        #region Fields

        public string OverrideBootstrapperScenePath = string.Empty;

        public List<string> LoadedScenes = new();

        public SelectedObjectPath[] SelectedObjects = Array.Empty<SelectedObjectPath>();

        #endregion
    }
}