using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    public class BootstrapStatusWindow : EditorWindow
    {
        private HelpBox _noActiveServicesBox;

        private InspectorElement _inspectorElement;

        private SerializedObject _serializedObject;

        [MenuItem("Window/General/Bootstrap Status Window")]
        private static void ShowWindow()
        {
            var window = GetWindow<BootstrapStatusWindow>();
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
            _noActiveServicesBox = new HelpBox(
                "There is no active services object in memory.",
                HelpBoxMessageType.Info);
            rootVisualElement.Add(_noActiveServicesBox);
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
            _inspectorElement = new InspectorElement(_serializedObject);
            rootVisualElement.Add(_inspectorElement);
        }
    }
}