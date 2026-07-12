using BeardPhantom.Bootstrap.EditMode;
using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap.Editor.Settings
{
    public class BootstrapSettingsEditor
    {
        public event Action<SerializedObject> SerializedObjectChanged;

        private const string UxmlGuid = "cc2657753a54cb14396441d2393d3d8f";

        private readonly Button _reinitializeButton;

        private readonly VisualElement _tabViewContent;

        private UnityEditor.Editor _editor;

        private readonly VisualElement _rootElement;

        public BootstrapSettingsEditor(VisualElement rootElement)
        {
            _rootElement = rootElement;
            CompilationPipeline.compilationStarted += OnCompilationStarted;

            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(UxmlGuid));
            uxml.CloneTree(rootElement);
            VisualElement content = rootElement.Q("settings-content");
            _tabViewContent = content.Q(className: "tab-view-content");
            var projectToggle = content.Q<ToolbarToggle>("project-toggle");
            var userToggle = content.Q<ToolbarToggle>("user-toggle");

            _reinitializeButton = content.Q<Button>("reinitialize-button");
            _reinitializeButton.clicked += OnReinitializeButtonClicked;

            SetupTabViewTab(projectToggle, SettingsScope.Project, userToggle);
            SetupTabViewTab(userToggle, SettingsScope.User, projectToggle);
            BindToScope(SettingsScope.Project);
        }

        ~BootstrapSettingsEditor()
        {
            CompilationPipeline.compilationStarted -= OnCompilationStarted;
        }

        private static void OnSerializedObjectPropertyChanged(SerializedObject obj)
        {
            var settingsAsset = (IBootstrapEditorSettingsAsset)obj.targetObject;
            settingsAsset.Save();
            BootstrapLogLevel minLogLevel = BootstrapEditorSettingsUtility.GetValue(asset => asset.MinLogLevel);
            Logging.MinLogLevel = minLogLevel;

            if (App.TryGetInstance(out EditModeAppInstance editModeAppInstance))
            {
                editModeAppInstance.ReinitializeIfNecessary();
            }
        }

        private static void OnObjectFieldSelectorMouseDown(MouseDownEvent evt, ObjectField objectField)
        {
            BootstrapEditorUtility.PickAsset<GameObject>(result =>
            {
                objectField.value = result;
            });
        }

        private static void FixComponentObjectFields(VisualElement root)
        {
            root.Query<ObjectField>()
                .Where(of => typeof(Component).IsAssignableFrom(of.objectType))
                .ForEach(FixComponentObjectField);
        }

        private static void FixComponentObjectField(ObjectField objectField)
        {
            VisualElement selector = objectField.Q(className: ObjectField.selectorUssClassName);
            VisualElement selectorParent = selector.parent;
            selector.RemoveFromHierarchy();
            selector = new VisualElement();
            selector.AddToClassList(ObjectField.selectorUssClassName);
            selector.RegisterCallback<MouseDownEvent, ObjectField>(OnObjectFieldSelectorMouseDown, objectField);
            selectorParent.Add(selector);
        }

        private static void OnReinitializeButtonClicked()
        {
            if (!App.TryGetInstance(out EditModeAppInstance _))
            {
                return;
            }

            App.Quit();
            App.Deinitialize();
            App.Initialize<EditModeAppInstance>();
        }

        private void OnCompilationStarted(object obj)
        {
            _rootElement.SetEnabled(false);
        }

        private void SetupTabViewTab(ToolbarToggle toggle, SettingsScope ownedScope, ToolbarToggle otherToggle)
        {
            toggle.RegisterValueChangedCallback(evt =>
            {
                if (!evt.newValue)
                {
                    // With current setup this can only happen if user clicked the active tab
                    toggle.SetValueWithoutNotify(true);
                    return;
                }

                otherToggle.SetValueWithoutNotify(false);
                BindToScope(ownedScope);
            });
        }

        private void BindToScope(SettingsScope bindingScope)
        {
            IBootstrapEditorSettingsAsset settingsAsset = bindingScope == SettingsScope.Project
                ? BootstrapEditorProjectSettings.instance
                : BootstrapEditorUserSettings.instance;

            var asset = (Object)settingsAsset;
            _tabViewContent.Unbind();
            for (int i = _tabViewContent.childCount - 1; i >= 1; i--)
            {
                _tabViewContent.RemoveAt(i);
            }

            _tabViewContent.Q<Label>("scope-label").text = bindingScope.ToString();

            UnityEditor.Editor.CreateCachedEditor(asset, null, ref _editor);

            SerializedObject serializedObject = _editor.serializedObject;

            InspectorElement.FillDefaultInspector(_tabViewContent, serializedObject, _editor);
            _tabViewContent.Bind(serializedObject);
            _tabViewContent.TrackSerializedObjectValue(serializedObject, OnSerializedObjectPropertyChanged);
            _tabViewContent.RemoveAt(1);
            FixComponentObjectFields(_tabViewContent);

            if (bindingScope == SettingsScope.Project)
            {
                _tabViewContent.Query<SettingsPropertyField>()
                    .ForEach(spf => spf.SetAlwaysOverride(true));
            }

            SerializedObjectChanged?.Invoke(serializedObject);
        }
    }
}