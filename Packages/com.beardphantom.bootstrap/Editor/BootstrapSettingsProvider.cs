using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ObjectField = UnityEditor.UIElements.ObjectField;
using SettingsProvider = UnityEditor.SettingsProvider;

namespace BeardPhantom.Bootstrap.Editor
{
    public class BootstrapSettingsProvider : SettingsProvider
    {
        public const string SettingsPath = "Project/Bootstrap";

        private const string UxmlGuid = "cc2657753a54cb14396441d2393d3d8f";

        private VisualElement _content;

        private SerializedObject _serializedObject;

        private VisualElement _tabViewContent;

        private UnityEditor.Editor _editor;

        private ToolbarToggle _projectToggle;

        private ToolbarToggle _userToggle;

        private ObjectField _servicesInstance;

        /// <inheritdoc />
        private BootstrapSettingsProvider() : base(SettingsPath, SettingsScope.Project) { }

        [SettingsProvider]
        private static SettingsProvider Open()
        {
            return new BootstrapSettingsProvider();
        }

        private static void OnSerializedObjectPropertyChanged(SerializedObject obj)
        {
            var settingsAsset = (IBootstrapEditorSettingsAsset)obj.targetObject;
            settingsAsset.Save();
            EditModeBootstrapping.PerformBootstrappingIfNecessary();
        }

        private static void OnObjectFieldSelectorMouseDown(MouseDownEvent evt, ObjectField objectField)
        {
            SearchService.ShowObjectPicker(
                (result, wasCanceled) =>
                {
                    if (wasCanceled)
                    {
                        return;
                    }

                    objectField.value = result;
                },
                default,
                $"p: prefab:any t:{nameof(EditModeServices)}",
                nameof(EditModeServices),
                typeof(GameObject));
        }

        private static void FixComponentObjectFields(VisualElement root)
        {
            root.Query<ObjectField>()
                .Where(of => typeof(Component).IsAssignableFrom(of.objectType))
                .ForEach(FixComponentObjectField);
        }

        private static void FixComponentObjectField(ObjectField objectField)
        {
            var selector = objectField.Q(className: ObjectField.selectorUssClassName);
            var selectorParent = selector.parent;
            selector.RemoveFromHierarchy();
            selector = new VisualElement();
            selector.AddToClassList(ObjectField.selectorUssClassName);
            selector.RegisterCallback<MouseDownEvent, ObjectField>(OnObjectFieldSelectorMouseDown, objectField);
            selectorParent.Add(selector);
        }

        /// <inheritdoc />
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            App.AppBootstrapStateChanged += OnBootstrapStateChanged;

            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(UxmlGuid));
            uxml.CloneTree(rootElement);
            _content = rootElement.Q("settings-content");
            _tabViewContent = _content.Q(className: "tab-view-content");
            _projectToggle = _content.Q<ToolbarToggle>("project-toggle");
            _userToggle = _content.Q<ToolbarToggle>("user-toggle");
            _servicesInstance = _content.Q<ObjectField>("services-instance");
            _servicesInstance.SetEnabled(false);
            SetupTabViewTab(_projectToggle, SettingsScope.Project, _userToggle);
            SetupTabViewTab(_userToggle, SettingsScope.User, _projectToggle);
            BindToScope(SettingsScope.Project);
            UpdateVolatileUI();
        }

        /// <inheritdoc />
        public override void OnDeactivate()
        {
            App.AppBootstrapStateChanged -= OnBootstrapStateChanged;
        }

        private void OnBootstrapStateChanged(AppBootstrapState previousState, AppBootstrapState newState)
        {
            UpdateVolatileUI();
        }

        private void UpdateVolatileUI()
        {
            EditModeBootstrapping.TryGetServicesInstance(out var instance);
            _servicesInstance.SetValueWithoutNotify(instance);
        }

        private void SetupTabViewTab(ToolbarToggle toggle, SettingsScope ownedScope, ToolbarToggle otherToggle)
        {
            toggle.RegisterValueChangedCallback(
                evt =>
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
            for (var i = _tabViewContent.childCount - 1; i >= 1; i--)
            {
                _tabViewContent.RemoveAt(i);
            }

            _tabViewContent.Q<Label>("scope-label").text = bindingScope.ToString();

            UnityEditor.Editor.CreateCachedEditor(asset, default, ref _editor);

            var serializedObject = _editor.serializedObject;
            keywords = GetSearchKeywordsFromSerializedObject(serializedObject);

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
        }
    }
}