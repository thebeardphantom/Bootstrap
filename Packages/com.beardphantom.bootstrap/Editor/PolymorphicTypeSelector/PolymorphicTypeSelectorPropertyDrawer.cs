using BeardPhantom.Bootstrap;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(PolymorphicTypeSelectorAttribute))]
public class PolymorphicTypeSelectorPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        return new StatefulElement(
            (PolymorphicTypeSelectorAttribute)attribute,
            property);
    }

    private class StatefulElement : VisualElement
    {
        private static readonly GUID s_stylesheetGuid = new("95843a8ba54d4d139dbcff8d6d29704c");

        private readonly Type _baseType;

        private readonly SerializedProperty _property;

        private readonly string _createNewButtonText;

        private Button _deleteButton;

        private Rect _createNewButtonRepaintRect;

        public StatefulElement(
            PolymorphicTypeSelectorAttribute attribute,
            SerializedProperty property)
        {
            _property = property.Copy();
            _baseType = attribute.BaseType;
            name = "root";
            _createNewButtonText = $"Create new {_baseType.Name}";
            var styleSheet = AssetDatabase.LoadAssetByGUID<StyleSheet>(s_stylesheetGuid);
            styleSheets.Add(styleSheet);
            RebuildUI();
        }

        private string GetLabel()
        {
            object managedReferenceValue = _property.managedReferenceValue;
            if (managedReferenceValue == null)
            {
                return _property.displayName;
            }

            string typeName = managedReferenceValue.GetType().Name;
            typeName = ObjectNames.NicifyVariableName(typeName);
            return $"{_property.displayName} ({typeName})";
        }

        private void RebuildUI()
        {
            Clear();

            string label = GetLabel();

            if (_property.managedReferenceValue == null)
            {
                UnregisterCallback<MouseEnterEvent>(OnMouseEnterRoot);
                UnregisterCallback<MouseLeaveEvent>(OnMouseLeaveRoot);
                var buttonField = IMGUIField.Create(label, OnDrawCreateNewButtonIMGUI);
                buttonField.name = "create-button";
                Add(buttonField);
            }
            else
            {
                RegisterCallback<MouseEnterEvent>(OnMouseEnterRoot);
                RegisterCallback<MouseLeaveEvent>(OnMouseLeaveRoot);
                var propertyField = new PropertyField(_property, label);
                propertyField.TrackPropertyValue(_property, OnSerializedPropertyValueChanged);
                Add(propertyField);

                _deleteButton = new Button
                {
                    text = "X",
                    name = "delete-button",
                    style = { display = DisplayStyle.None, },
                };
                _deleteButton.AddToClassList("delete-button");
                _deleteButton.clickable.clickedWithEventInfo += OnDeleteButtonClicked;
                Add(_deleteButton);
            }

            this.Bind(_property.serializedObject);
        }

        private void OnDrawCreateNewButtonIMGUI()
        {
            GUIContent content = EditorGUIUtility.TrTempContent(_createNewButtonText);
            Rect createNewButtonRect = GUILayoutUtility.GetRect(content, GUI.skin.button);
            if (Event.current.type == EventType.Repaint)
            {
                _createNewButtonRepaintRect = createNewButtonRect;
            }

            if (!GUI.Button(createNewButtonRect, content))
            {
                return;
            }

            Rect dropdownRect = _createNewButtonRepaintRect;
            var dropdown = new PolymorphicTypeSelectorDropdown(_baseType);
            dropdown.ComponentTypeSelected += OnComponentTypeSelected;
            dropdown.Show(dropdownRect);
        }

        private void OnMouseEnterRoot(MouseEnterEvent _)
        {
            _deleteButton.style.display = DisplayStyle.Flex;
        }

        private void OnMouseLeaveRoot(MouseLeaveEvent _)
        {
            _deleteButton.style.display = DisplayStyle.None;
        }

        private void OnDeleteButtonClicked(EventBase obj)
        {
            _property.managedReferenceValue = null;
            _property.serializedObject.ApplyModifiedProperties();
        }

        private void OnSerializedPropertyValueChanged(SerializedProperty obj)
        {
            RebuildUI();
        }

        private void OnComponentTypeSelected(Type type)
        {
            object instance = Activator.CreateInstance(type);
            _property.managedReferenceValue = instance;
            _property.serializedObject.ApplyModifiedProperties();
            RebuildUI();
        }

        private class IMGUIField : BaseField<bool>
        {
            public readonly IMGUIContainer IMGUIContainer;

            private IMGUIField(string label, IMGUIContainer buttonGUIContainer) : base(label, buttonGUIContainer)
            {
                AddToClassList(alignedFieldUssClassName);
                buttonGUIContainer.style.flexGrow = 1f;
                buttonGUIContainer.style.marginTop = 0f;
                buttonGUIContainer.style.marginBottom = 0f;
            }

            public static IMGUIField Create(string label, Action drawButtonGui)
            {
                return new IMGUIField(label, new IMGUIContainer(drawButtonGui));
            }
        }
    }
}