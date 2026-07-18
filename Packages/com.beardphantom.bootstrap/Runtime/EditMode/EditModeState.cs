#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEditor;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Snapshot of editor state captured before entering play mode, so it can be restored when returning to
    /// edit mode.
    /// </summary>
    [Serializable]
    public class EditModeState
    {
        /// <summary>
        /// The scene paths that were loaded when the snapshot was taken.
        /// </summary>
        public List<string> LoadedScenes { get; set; } = new();

        /// <summary>
        /// The hierarchy paths of the objects that were selected when the snapshot was taken.
        /// </summary>
        public SelectedObjectPath[] SelectedObjects { get; set; } = Array.Empty<SelectedObjectPath>();

        /// <summary>
        /// The environment that was active when the snapshot was taken.
        /// </summary>
        [JsonIgnore]
        public BootstrapEnvironmentAsset Environment { get; set; }

        /// <summary>
        /// The asset GUID of <see cref="Environment"/>, used to resolve it across serialization.
        /// </summary>
        public string EnvironmentGuid { get; set; }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(EnvironmentGuid);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                return;
            }

            Environment = AssetDatabase.LoadAssetAtPath<BootstrapEnvironmentAsset>(assetPath);
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            if (!Environment)
            {
                EnvironmentGuid = null;
                return;
            }

            EnvironmentGuid = AssetDatabase.TryGetGUIDAndLocalFileIdentifier(Environment, out string guid, out _) ? guid : null;
        }
    }
}
#endif