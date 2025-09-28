using BeardPhantom.Bootstrap;
using System;
using System.Reflection;
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
            property.serializedObject,
            property.propertyPath,
            fieldInfo);
    }

    private class StatefulElement : VisualElement
    {
        private static readonly GUID s_stylesheetGuid = new("95843a8ba54d4d139dbcff8d6d29704c");

        private readonly Type _baseType;

        private readonly string _propertyPath;
        
        private readonly FieldInfo _fieldInfo;

        private readonly SerializedObject _serializedObject;

        private SerializedProperty _property;

        private Button _deleteButton;

        public StatefulElement(
            PolymorphicTypeSelectorAttribute attribute,
            SerializedObject serializedObject,
            string propertyPath,
            FieldInfo fieldInfo)
        {
            _serializedObject = serializedObject;
            _propertyPath = propertyPath;
            _fieldInfo = fieldInfo;
            _baseType = attribute.BaseType;
            name = "root";
            var styleSheet = AssetDatabase.LoadAssetByGUID<StyleSheet>(s_stylesheetGuid);
            styleSheets.Add(styleSheet);
            RebuildUI();
        }

        private static string GetLabel(SerializedProperty componentProperty)
        {
            object managedReferenceValue = componentProperty.managedReferenceValue;
            if (managedReferenceValue == null)
            {
                return componentProperty.displayName;
            }

            string name = managedReferenceValue.GetType().Name;
            return ObjectNames.NicifyVariableName(name);
        }

        private void RebuildUI()
        {
            Clear();
            _property = _serializedObject.FindProperty(_propertyPath);

            string label = GetLabel(_property);
            if (_property.managedReferenceValue == null)
            {
                UnregisterCallback<MouseEnterEvent>(OnMouseEnterRoot);
                UnregisterCallback<MouseLeaveEvent>(OnMouseLeaveRoot);

                var createButton = new Button
                {
                    text = $"Create New {_baseType.Name}",
                    name = "create-button",
                };
                createButton.AddToClassList("create-button");
                createButton.clickable.clickedWithEventInfo += OnCreateNewButtonClicked;
                Add(createButton);
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

            this.Bind(_serializedObject);
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
            _serializedObject.ApplyModifiedProperties();
        }

        private void OnSerializedPropertyValueChanged(SerializedProperty obj)
        {
            RebuildUI();
        }

        private void OnCreateNewButtonClicked(EventBase obj)
        {
            var target = (Button)obj.currentTarget;
            Rect rect = target.worldBound;
            // rect.position = obj.originalMousePosition;
            var dropdown = new PolymorphicClassSelectorDropdown(_baseType);
            dropdown.ComponentTypeSelected += OnComponentTypeSelected;
            dropdown.Show(rect);
        }

        private void OnComponentTypeSelected(Type type)
        {
            object instance = Activator.CreateInstance(type);
            _property.managedReferenceValue = instance;
            _serializedObject.ApplyModifiedProperties();
            RebuildUI();
        }
    }
}