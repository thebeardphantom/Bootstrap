using System;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        public string[] LoadedScenes;

        public SelectedObjectPath[] SelectedObjects;

        public string BootstrapperJson;
    }
}