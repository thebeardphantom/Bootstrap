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

        private Label _sessionGuidTextField;

        private Label _appCreateTimestampTextField;

        private ObjectField _sourceAsset;

        private Label _appType;

        [MenuItem("Window/General/Bootstrap Status Window")]
        public static void ShowWindow()
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

            _sessionGuidTextField = rootVisualElement.Q<Label>("session-guid");
            _appCreateTimestampTextField = rootVisualElement.Q<Label>("app-create-timestamp");
            _noActiveServicesBox = rootVisualElement.Q<HelpBox>();
            _sectionServices = rootVisualElement.Q<Foldout>("section-services");
            _sourceAsset = rootVisualElement.Q<ObjectField>("source-asset");
            _appType = rootVisualElement.Q<Label>("app-type");
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

        private void OnAddedAsTab()
        {
            RefreshUI();
        }

        private void OnTabDetached()
        {
            RefreshUI();
        }

        private void RefreshUI()
        {
            _inspectorElement?.RemoveFromHierarchy();
            _serializedObject?.Dispose();
            _inspectorElement = null;
            _serializedObject = null;
            _noActiveServicesBox.style.display = DisplayStyle.Flex;
            _sourceAsset.SetValueWithoutNotify(null);

            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                _appType.text = null;
                _sessionGuidTextField.text = null;
                _appCreateTimestampTextField.text = null;
                return;
            }

            _appType.text = appInstance.GetType().Name;
            _appType.tooltip = appInstance.GetType().ToString();
            _sessionGuidTextField.text = appInstance.SessionGuid.ToString();
            _appCreateTimestampTextField.text = appInstance.CreateTimestamp.ToString();

            if (appInstance.ActiveServiceList.IsNull())
            {
                return;
            }

            _sourceAsset.SetValueWithoutNotify(appInstance.ActiveServiceList.Source);
            _noActiveServicesBox.style.display = DisplayStyle.None;
            _serializedObject = new SerializedObject(appInstance.ActiveServiceList);
            _inspectorElement = new InspectorElement(_serializedObject)
            {
                name = "services-inspector",
            };
            _inspectorElement.RegisterCallbackOnce<AttachToPanelEvent>(OnInspectorElementAttachedToPanel);
            _sectionServices.Add(_inspectorElement);
        }

        private void OnInspectorElementAttachedToPanel(AttachToPanelEvent _)
        {
            var scriptField = _inspectorElement.Q<PropertyField>("PropertyField:m_Script");
            scriptField.style.display = DisplayStyle.None;

            var listView = _inspectorElement.Q<ListView>();
            listView.showAddRemoveFooter = false;
            listView.showBoundCollectionSize = false;
            listView.reorderable = false;
            listView.allowRemove = false;
            listView.allowAdd = false;
            listView.selectionType = SelectionType.None;
            listView.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                listView.reorderable = false;
            });
        }
    }
}