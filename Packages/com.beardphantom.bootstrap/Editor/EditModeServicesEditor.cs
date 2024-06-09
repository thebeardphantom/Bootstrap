using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    [CustomEditor(typeof(EditModeServices))]
    public class EditModeServicesEditor : UnityEditor.Editor
    {
        #region Fields

        private const string UXML_GUID = "a6549777747b40899a8ce75ecc5547ea";

        private const string USER_OVERRIDE = "User Override";

        private const string PROJECT_DEFAULT = "Project Default";

        private EditModeServices _editModeServicesSrc;

        private ToolbarButton _makeActiveButton;

        private ToolbarButton _selectActiveButton;

        private ToolbarButton _settingsButton;

        #endregion

        #region Methods

        private static void OnSettingsButtonClicked()
        {
            SettingsService.OpenProjectSettings(BootstrapSettingsProvider.SETTINGS_PATH);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _editModeServicesSrc = (EditModeServices)target;

            var root = new VisualElement();

            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(UXML_GUID));
            uxml.CloneTree(root);

            var sourcePrefab = root.Q<ObjectField>("source-prefab");
            var toolbar = root.Q<Toolbar>("toolbar");
            _makeActiveButton = toolbar.Q<ToolbarButton>("make-active-button");
            _selectActiveButton = toolbar.Q<ToolbarButton>("select-active-button");
            _settingsButton = toolbar.Q<ToolbarButton>("settings-button");

            _selectActiveButton.clicked += OnSelectActiveButtonClicked;
            _settingsButton.clicked += OnSettingsButtonClicked;

            if (PrefabUtility.IsPartOfAnyPrefab(_editModeServicesSrc))
            {
                sourcePrefab.AddToClassList("no-display");
                _makeActiveButton.clicked += OnMakeActiveButtonClicked;
            }
            else if (BootstrapUtility.IsPartOfPrefabStage(_editModeServicesSrc))
            {
                sourcePrefab.AddToClassList("no-display");
            }
            else
            {
                _makeActiveButton.SetEnabled(false);
                sourcePrefab.SetValueWithoutNotify(_editModeServicesSrc.SourcePrefab);
                sourcePrefab.SetEnabled(false);
            }

            InspectorElement.FillDefaultInspector(root, serializedObject, this);
            return root;
        }

        private void OnMakeActiveButtonClicked()
        {
            var menu = new GenericMenu();

            var userSettings = BootstrapEditorUserSettings.instance;
            var userServices = userSettings.EditModeServices;
            var isUser = userServices.Value == _editModeServicesSrc;
            menu.AddItem(
                new GUIContent(USER_OVERRIDE),
                isUser,
                () =>
                {
                    if (isUser)
                    {
                        userServices.OverrideEnabled = false;
                        userServices.SetValue(default);
                    }
                    else
                    {
                        userServices.OverrideEnabled = true;
                        userServices.SetValue(_editModeServicesSrc);
                    }

                    userSettings.Save();
                });

            var projectSettings = BootstrapEditorProjectSettings.instance;
            var projectServices = projectSettings.EditModeServices;
            var isProject = projectServices.Value == _editModeServicesSrc;
            menu.AddItem(
                new GUIContent(PROJECT_DEFAULT),
                isProject,
                () =>
                {
                    projectServices.SetValue(
                        isProject
                            ? default
                            : _editModeServicesSrc);
                    projectSettings.Save();
                });

            menu.ShowAsContext();
        }

        private void OnSelectActiveButtonClicked()
        {
            var menu = new GenericMenu();
            var userServices = BootstrapEditorUserSettings.instance.EditModeServices;
            var isUser = userServices.Value == _editModeServicesSrc;
            if (userServices.Value == null)
            {
                menu.AddDisabledItem(new GUIContent(USER_OVERRIDE));
            }
            else
            {
                menu.AddItem(
                    new GUIContent(USER_OVERRIDE),
                    isUser,
                    () =>
                    {
                        var services = userServices.Value;
                        if (services != null)
                        {
                            Selection.activeObject = services;
                        }
                    });
            }


            var projectServices = BootstrapEditorProjectSettings.instance.EditModeServices;
            var isProject = projectServices.Value == _editModeServicesSrc;
            if (projectServices.Value == null)
            {
                menu.AddDisabledItem(new GUIContent(PROJECT_DEFAULT));
            }
            else
            {
                menu.AddItem(
                    new GUIContent(PROJECT_DEFAULT),
                    isProject,
                    () =>
                    {
                        var services = projectServices.Value;
                        if (services != null)
                        {
                            Selection.activeObject = services;
                        }
                    });
            }

            menu.ShowAsContext();
        }

        #endregion
    }
}