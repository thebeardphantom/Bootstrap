#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;

namespace BeardPhantom.Bootstrap
{
    [Serializable]
    public class EditModeState
    {
        public List<string> LoadedScenes { get; set; } = new();

        public SelectedObjectPath[] SelectedObjects { get; set; } = Array.Empty<SelectedObjectPath>();

        [JsonIgnore]
        public RuntimeBootstrapEnvironmentAsset Environment { get; set; }

        public string EnvironmentGuid { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(EnvironmentGuid);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return;
            }

            Environment = AssetDatabase.LoadAssetAtPath<RuntimeBootstrapEnvironmentAsset>(assetPath);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (Environment == null)
            {
                EnvironmentGuid = default;
                return;
            }

            EnvironmentGuid = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Environment, out string guid, out _) ? guid : default;
        }
    }
}
#endif