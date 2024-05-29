using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor
{
    [CustomEditor(typeof(EditModeServices))]
    public class EditModeServicesEditor : UnityEditor.Editor
    {
        #region Fields

        private ToolbarButton _setActiveButton;

        private ToolbarButton _selectActiveButton;

        private EditModeServices _editModeServicesSrc;

        #endregion

        #region Methods

        private static void OnSelectActiveButtonClicked()
        {
            if (EditorBootstrapSingleton.instance.TryGetActiveEditModeServices(out var activeEditModeServices))
            {
                EditorGUIUtility.PingObject(activeEditModeServices);
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            _editModeServicesSrc = (EditModeServices)target;
            if (PrefabUtility.IsPartOfAnyPrefab(_editModeServicesSrc))
            {
                EditorBootstrapSingleton.instance.TryGetActiveEditModeServices(out var activeEditModeServices);
                root.Add(
                    new ObjectField("Active Services")
                    {
                        objectType = typeof(EditModeServices),
                        value = activeEditModeServices
                    });
                // var toolbar = new Toolbar();
                // _editModeServicesSrc = BootstrapUtility.GetSourceObject(_editModeServicesSrc);
                // _setActiveButton = new ToolbarButton
                // {
                //     text = "Set Active"
                // };
                // _setActiveButton.clicked += OnSetActiveButtonClicked;
                // toolbar.Add(_setActiveButton);
                //
                // _selectActiveButton = new ToolbarButton
                // {
                //     text = "Select Active"
                // };
                // _selectActiveButton.clicked += OnSelectActiveButtonClicked;
                // toolbar.Add(_selectActiveButton);
                ConfigureButtons();
                // root.Insert(0, toolbar);
            }
            else if (!BootstrapUtility.IsPartOfPrefabStage(_editModeServicesSrc))
            {
                var objectField = new ObjectField("Source Prefab")
                {
                    value = _editModeServicesSrc.SourcePrefab
                };
                objectField.SetEnabled(false);
                root.Add(objectField);
            }

            return root;
        }

        private void OnSetActiveButtonClicked()
        {
            EditorBootstrapSingleton.instance.SetActiveEditModeServices(_editModeServicesSrc);
            EditModeBootstrapper.PerformBootstrapping().Forget();
            ConfigureButtons();
        }

        private void ConfigureButtons()
        {
            // EditorBootstrapSingleton.instance.TryGetActiveEditModeServices(out var activeEditModeServices);
            // var isActive = activeEditModeServices == _editModeServicesSrc;
            // if (isActive)
            // {
            //     const string TOOLTIP = "This is already the active edit mode services instance.";
            //     _setActiveButton.tooltip = TOOLTIP;
            //     _selectActiveButton.tooltip = TOOLTIP;
            // }
            // else
            // {
            //     _setActiveButton.tooltip = default;
            //     _selectActiveButton.tooltip = default;
            // }
            //
            // _setActiveButton.SetEnabled(!isActive);
            // _selectActiveButton.SetEnabled(!isActive);
        }

        #endregion
    }
}