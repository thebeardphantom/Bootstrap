using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    public class BootstrapAppWindow : EditorWindow
    {
        private static readonly GUID s_uxmlGuid = new("9ae124f587507a1469f8c82e44b628de");

        private HelpBox _noActiveServicesBox;

        private InspectorElement _inspectorElement;

        private SerializedObject _serializedObject;
        
        private Foldout _sectionServices;

        [MenuItem("Window/General/Bootstrap Status Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<BootstrapAppWindow>();
            window.titleContent = new GUIContent("🥾Bootstrap Status");
            window.Show();
        }

        private void OnEnable()
        {
            App.Deinitialized -= OnAppDeinitialized;
            App.Deinitialized += OnAppDeinitialized;

            App.Initialized -= OnAppInitialized;
            App.Initialized += OnAppInitialized;
        }

        private void OnDisable()
        {
            App.Deinitialized -= OnAppDeinitialized;
            App.Initialized -= OnAppInitialized;
        }

        private void CreateGUI()
        {
            var visualTreeAsset = AssetDatabase.LoadAssetByGUID<VisualTreeAsset>(s_uxmlGuid);
            visualTreeAsset.CloneTree(rootVisualElement);

            _noActiveServicesBox = rootVisualElement.Q<HelpBox>();
            _sectionServices = rootVisualElement.Q<Foldout>("section-services");
            RefreshUI();
        }

        private void OnAppInitialized()
        {
            RefreshUI();
        }

        private void OnAppDeinitialized()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            _inspectorElement?.RemoveFromHierarchy();
            _inspectorElement = null;
            _serializedObject = null;
            _noActiveServicesBox.style.display = DisplayStyle.Flex;

            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                return;
            }

            if (appInstance.ActiveServiceListAsset == null)
            {
                return;
            }

            _noActiveServicesBox.style.display = DisplayStyle.None;
            _serializedObject = new SerializedObject(appInstance.ActiveServiceListAsset);
            _inspectorElement = new InspectorElement(_serializedObject)
            {
                name = "services-inspector",
            };
            _sectionServices.Add(_inspectorElement);
        }
    }
}