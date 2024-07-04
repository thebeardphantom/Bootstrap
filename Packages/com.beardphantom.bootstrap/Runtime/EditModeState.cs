using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        public List<string> LoadedScenes { get; set; } = new();

        public SelectedObjectPath[] SelectedObjects { get; set; } = Array.Empty<SelectedObjectPath>();
    }
}