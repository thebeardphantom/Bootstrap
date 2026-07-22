using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assertions;
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
        [field: Tooltip("If true, adds the bootstrap scene to the build settings scene list at index 0.")]
        private bool ModifyScenesList { get; set; } = true;

        /// <summary>
        /// If true, creates a nEnvironment asset and assigns it as the default for play mode and builds..
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates an Environment asset and assigns it as the default for play mode and builds.")]
        private bool CreateDefaultEnvironment { get; set; } = true;

        /// <summary>
        /// If true, creates a ServiceList asset. If also creating a default environment it will use this ServiceList.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates a ServiceList asset. If also creating a default environment it will use this ServiceList.")]
        private bool CreateRuntimeServiceList { get; set; } = true;

        /// <summary>
        /// If true, creates the edit mode service list asset and assigns it in editor settings.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("If true, creates the edit mode service list asset and assigns it in editor settings.")]
        private bool CreateEditModeServiceList { get; set; } = true;

        [MenuItem("Edit/Bootstrap Wizard")]
        private static void Open()
        {
            DisplayWizard<BootstrapWizard>("Bootstrap Wizard", "Run Setup");
        }

        private void OnWizardCreate()
        {
            Scene bootstrapScene = default;
            Scene activeScene = SceneManager.GetActiveScene();
            try
            {
                Directory.CreateDirectory(OutputDirectory);

                if (CreateBootstrapScene)
                {
                    bootstrapScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                    SceneManager.SetActiveScene(bootstrapScene);
                }

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
                Assert.IsTrue(bootstrapScene.IsValid(), "bootstrapScene.IsValid() == false");
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

            BootstrapEnvironmentAsset environmentAsset = null;
            if (CreateDefaultEnvironment)
            {
                environmentAsset = CreateInstance<BootstrapEnvironmentAsset>();
                environmentAsset.name = EnvironmentName;
                AssetDatabase.CreateAsset(environmentAsset, EnvironmentPath);

                IBootstrapEditorSettingsAsset settings = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.Project);
                settings.DefaultPlayModeEnvironment.SetValue(environmentAsset);
                settings.DefaultBuildEnvironment.SetValue(environmentAsset);
            }

            if (CreateRuntimeServiceList)
            {
                var servicesListAsset = CreateInstance<ServiceList>();
                servicesListAsset.name = ServiceListAssetName;
                AssetDatabase.CreateAsset(servicesListAsset, ServiceListAssetPath);
                if (CreateDefaultEnvironment)
                {
                    Assert.IsNotNull(environmentAsset, "environmentAsset == null");
                    environmentAsset.ServiceListAsset = servicesListAsset;
                }
            }
        }

        private void CreateEditModeAssets()
        {
            if (!CreateEditModeServiceList)
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