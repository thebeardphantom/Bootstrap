using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        #region Fields

        public List<string> LoadedScenes;

        public SelectedObjectPath[] SelectedObjects;

        #endregion
    }
}