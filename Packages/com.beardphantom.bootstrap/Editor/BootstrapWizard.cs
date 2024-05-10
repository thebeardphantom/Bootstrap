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
        #region Properties

        [field: Delayed]
        [field: SerializeField]
        private string OutputDirectory { get; set; } = "Assets/Bootstrap";

        [field: Delayed]
        [field: SerializeField]
        private string SceneName { get; set; } = "Bootstrap";

        [field: Delayed]
        [field: SerializeField]
        private string ServicesPrefabName { get; set; } = "Services";

        [field: SerializeField]
        private bool ServicesPrefabInRoot { get; set; } = true;

        private string ScenePath => $"{OutputDirectory}{SceneName}.unity";

        private string BootstrapPrefabPath => $"{OutputDirectory}{Bootstrapper.BOOTSTRAP_GAMEOBJECT_NAME}.prefab";

        private string ServicesPrefabPath => ServicesPrefabInRoot
            ? $"Assets/{ServicesPrefabName}.prefab"
            : $"{OutputDirectory}{ServicesPrefabName}.prefab";

        #endregion

        #region Methods

        [MenuItem("Edit/Bootstrap Wizard")]
        private static void Open()
        {
            DisplayWizard<BootstrapWizard>("Bootstrap Wizard", "Run Setup");
        }

        /// <inheritdoc />
        protected override bool DrawWizardGUI()
        {
            using var serializedObject = new SerializedObject(this);
            var serializedProperty = serializedObject.GetIterator();

            // Enter first property and skip m_Script and m_SerializedDataModeController
            serializedProperty.NextVisible(true);
            serializedProperty.NextVisible(false);
            serializedProperty.NextVisible(false);
            do
            {
                // if(serializedProperty.propertyPath is not "m_Script" or "")
                EditorGUILayout.PropertyField(serializedProperty, true);
            }
            while (serializedProperty.NextVisible(false));

            DrawOutputPaths();

            return serializedObject.ApplyModifiedProperties();
        }

        private void DrawOutputPaths()
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Output Paths", EditorStyles.boldLabel);

            const string ASSET_EXISTS_WARN = "Asset exists at this path and will be overwritten.";

            var assetPathExists = AssetDatabase.AssetPathExists(ScenePath);
            GUI.contentColor = assetPathExists ? Color.yellow : Color.white;
            var tooltip = assetPathExists ? ASSET_EXISTS_WARN : null;
            EditorGUILayout.LabelField(new GUIContent("Scene Path", tooltip), new GUIContent(ScenePath, tooltip));

            assetPathExists = AssetDatabase.AssetPathExists(BootstrapPrefabPath);
            GUI.contentColor = assetPathExists ? Color.yellow : Color.white;
            tooltip = assetPathExists ? ASSET_EXISTS_WARN : null;
            EditorGUILayout.LabelField(
                new GUIContent("Bootstrap Prefab Path", tooltip),
                new GUIContent(BootstrapPrefabPath, tooltip));

            assetPathExists = AssetDatabase.AssetPathExists(ServicesPrefabPath);
            GUI.contentColor = assetPathExists ? Color.yellow : Color.white;
            tooltip = assetPathExists ? ASSET_EXISTS_WARN : null;
            EditorGUILayout.LabelField(
                new GUIContent("Services Prefab Path", tooltip),
                new GUIContent(ServicesPrefabPath, tooltip));

            GUI.contentColor = Color.white;
        }

        private void OnWizardUpdate()
        {
            FixPathsAndNames();
        }

        private void FixPathsAndNames()
        {
            if (!OutputDirectory.StartsWith("Assets/"))
            {
                OutputDirectory = $"Assets/{OutputDirectory}";
            }

            if (!OutputDirectory.EndsWith("/"))
            {
                OutputDirectory = $"{OutputDirectory}/";
            }

            if (string.IsNullOrEmpty(SceneName))
            {
                SceneName = "Bootstrap";
            }

            if (string.IsNullOrEmpty(ServicesPrefabName))
            {
                ServicesPrefabName = "Services";
            }
        }

        private void OnEnable()
        {
            FixPathsAndNames();
        }

        private void OnWizardCreate()
        {
            Scene bootstrapScene = default;
            try
            {
                Directory.CreateDirectory(OutputDirectory);

                bootstrapScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                var didSave = EditorSceneManager.SaveScene(bootstrapScene, ScenePath);
                if (!didSave)
                {
                    Debug.LogError("Bootstrap scene not saved, cancelling asset creation.");
                    return;
                }

                var servicesObj = new GameObject(ServicesPrefabName);
                var servicesPrefab = PrefabUtility.SaveAsPrefabAsset(servicesObj, ServicesPrefabPath, out var success);
                DestroyImmediate(servicesObj);
                if (!success)
                {
                    Debug.LogError("Services prefab not saved, cancelling asset creation.");
                    return;
                }

                var bootstrapperPrefab = new GameObject();
                SceneManager.MoveGameObjectToScene(bootstrapperPrefab, bootstrapScene);
                var bootstrapper = bootstrapperPrefab.AddComponent<Bootstrapper>();
                var prefabLoader = ServicesPrefabLoader.Create<DirectServicesPrefabLoader>(bootstrapperPrefab, servicesPrefab);
                bootstrapper.ServicesPrefabLoader = prefabLoader;
                
                PrefabUtility.SaveAsPrefabAssetAndConnect(
                    bootstrapperPrefab,
                    BootstrapPrefabPath,
                    InteractionMode.AutomatedAction,
                    out success);

                if (!success)
                {
                    Debug.LogError("Bootstrap prefab not saved.");
                }

                EditorSceneManager.SaveScene(bootstrapScene);

                var scenesList = EditorBuildSettings.scenes.ToList();
                scenesList.RemoveAll(a => a.path == ScenePath);
                scenesList.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = scenesList.ToArray();
            }
            finally
            {
                if (bootstrapScene.IsValid() && bootstrapScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(bootstrapScene, true);
                }
            }
        }

        #endregion
    }
}