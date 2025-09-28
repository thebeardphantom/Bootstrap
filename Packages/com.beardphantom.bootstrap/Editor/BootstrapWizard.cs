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
    public class BootstrapWizard : ScriptableWizard
    {
        private const string OutputDirectory = "Assets/Bootstrap/";

        private const string EnvironmentName = "Env_Default";

        private const string EnvironmentPath = OutputDirectory + EnvironmentName + ".asset";

        private const string BootstrapperPrefabName = EnvironmentName + "_Bootstrapper";

        private const string ServicesPrefabName = EnvironmentName + "_Services";

        private const string ServicesPrefabPath = OutputDirectory + ServicesPrefabName + ".prefab";

        private const string ScenePath = OutputDirectory + "Bootstrap.unity";

        private const string EditModeServicesPrefabName = "EditModeServices";

        private const string EditModeServicesPrefabPath = OutputDirectory + EditModeServicesPrefabName + ".prefab";

        [field: SerializeField]
        private bool CreateBootstrapScene { get; set; } = true;

        [field: SerializeField]
        private bool ModifyScenesList { get; set; } = true;

        [field: SerializeField]
        private bool CreateDefaultEnvironment { get; set; } = true;

        [field: SerializeField]
        private bool CreateEditModeServicesPrefab { get; set; } = true;

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
            if (!CreateEditModeServicesPrefab)
            {
                return;
            }

            var editModeServices = new GameObject(EditModeServicesPrefabName);
            GameObject editModeServicesPrefab = PrefabUtility.SaveAsPrefabAsset(
                editModeServices,
                EditModeServicesPrefabPath,
                out bool success);
            if (!success)
            {
                Logging.Error("EditModeServices prefab not saved.");
            }

            DestroyImmediate(editModeServices);
            IBootstrapEditorSettingsAsset settings = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.Project);
            settings.EditModeServices.SetValue(editModeServicesPrefab);
        }
    }
}