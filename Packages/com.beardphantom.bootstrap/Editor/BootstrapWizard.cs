using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap.Editor
{
    /// <summary>
    /// Editor wizard that scaffolds a new Bootstrap setup: creates the bootstrap scene, registers it in the build
    /// settings, and creates the default environment and edit mode service list assets.
    /// </summary>
    public class BootstrapWizard : ScriptableWizard
    {
        private const string OutputDirectory = "Assets/Bootstrap/";

        private const string EnvironmentName = "Env_Default";

        private const string EnvironmentPath = OutputDirectory + EnvironmentName + ".asset";

        private const string ServiceListAssetName = EnvironmentName + "_Services";

        private const string ServiceListAssetPath = OutputDirectory + ServiceListAssetName + ".asset";

        private const string ScenePath = OutputDirectory + "Bootstrap.unity";

        private const string EditModeServiceListAssetName = "EditModeServices";

        private const string EditModeServiceListAssetPath = OutputDirectory + EditModeServiceListAssetName + ".asset";

        /// <summary>
        /// If true, creates the bootstrap scene at the configured output path.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates the bootstrap scene at the configured output path.")]
        private bool CreateBootstrapScene { get; set; } = true;

        /// <summary>
        /// If true, adds the bootstrap scene to the build settings scene list.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, adds the bootstrap scene to the build settings scene list.")]
        private bool ModifyScenesList { get; set; } = true;

        /// <summary>
        /// If true, creates the default Bootstrap environment asset and assigns it in editor settings.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates the default Bootstrap environment asset and assigns it in editor settings.")]
        private bool CreateDefaultEnvironment { get; set; } = true;

        /// <summary>
        /// If true, creates the edit mode service list asset and assigns it in editor settings.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates the edit mode service list asset and assigns it in editor settings.")]
        private bool CreateEditModeServiceListAsset { get; set; } = true;

        [MenuItem("Edit/Bootstrap Wizard")]
        private static void Open()
        {
            DisplayWizard<BootstrapWizard>("Bootstrap Wizard", "Run Setup");
        }

        private static bool DoesAssetPathExist(string path)
        {
            return !string.IsNullOrWhiteSpace(AssetDatabase.AssetPathToGUID(path));
        }

        private void OnWizardCreate()
        {
            Scene bootstrapScene = default;
            Scene activeScene = SceneManager.GetActiveScene();
            try
            {
                Directory.CreateDirectory(OutputDirectory);

                bootstrapScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                SceneManager.SetActiveScene(bootstrapScene);

                CreateRuntimeAssets(bootstrapScene);
                CreateEditModeAssets();
            }
            finally
            {
                SceneManager.SetActiveScene(activeScene);
                if (bootstrapScene.IsValid() && bootstrapScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(bootstrapScene, true);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CreateRuntimeAssets(Scene bootstrapScene)
        {
            if (CreateBootstrapScene)
            {
                bool didSave = EditorSceneManager.SaveScene(bootstrapScene, ScenePath);
                if (!didSave)
                {
                    Logging.Error("Bootstrap scene not saved.");
                }
            }

            if (ModifyScenesList)
            {
                List<EditorBuildSettingsScene> scenesList = EditorBuildSettings.scenes.ToList();
                scenesList.RemoveAll(a => a.path == ScenePath);
                scenesList.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenesList.ToArray();
            }

            if (CreateDefaultEnvironment)
            {
                var environmentAsset = CreateInstance<BootstrapEnvironmentAsset>();
                environmentAsset.name = EnvironmentName;
                AssetDatabase.CreateAsset(environmentAsset, EnvironmentPath);

                var bootstrapSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
                IBootstrapEditorSettingsAsset settings = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.Project);
                settings.DefaultPlayModeEnvironment.SetValue(environmentAsset);
                settings.DefaultBuildEnvironment.SetValue(environmentAsset);
                settings.EditorSceneEnvironments.Value.AddOrReplace(bootstrapSceneAsset, environmentAsset);
            }
        }

        private void CreateEditModeAssets()
        {
            if (!CreateEditModeServiceListAsset)
            {
                return;
            }

            var servicesListAsset = CreateInstance<ServiceList>();
            AssetDatabase.CreateAsset(servicesListAsset, EditModeServiceListAssetPath);
            IBootstrapEditorSettingsAsset settings = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.Project);
            settings.EditModeServices.SetValue(servicesListAsset);
        }
    }
}