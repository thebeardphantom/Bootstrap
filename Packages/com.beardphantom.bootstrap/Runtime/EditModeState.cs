using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        public string BootstrapperJson;
        
        public List<string> LoadedScenes;

        public SelectedObjectPath[] SelectedObjects;
    }
}