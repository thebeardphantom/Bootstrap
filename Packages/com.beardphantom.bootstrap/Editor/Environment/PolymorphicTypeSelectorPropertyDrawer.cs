using BeardPhantom.Bootstrap;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(PolymorphicTypeSelectorAttribute))]
public class PolymorphicTypeSelectorPropertyDrawer : PropertyDrawer
{
    private static readonly GUID s_stylesheetGuid = new("0b3bfa1eefccb6740951e6b212c22097");

    private SerializedProperty _property;

    private VisualElement _root;

    private string _propertyPath;

    private SerializedObject _serializedObject;

    private static string GetLabel(SerializedProperty componentProperty)
    {
        object managedReferenceValue = componentProperty.managedReferenceValue;
        Debug.Assert(managedReferenceValue != null);

        string name = managedReferenceValue.GetType().Name;
        return ObjectNames.NicifyVariableName(name);
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        _serializedObject = property.serializedObject;
        _propertyPath = property.propertyPath;
        _root = new VisualElement
        {
            name = "root",
        };
        RebuildUI();

        // var styleSheet = AssetDatabase.LoadAssetByGUID<StyleSheet>(s_stylesheetGuid);
        // root.styleSheets.Add(styleSheet);

        return _root;
    }

    private void RebuildUI()
    {
        _root.Clear();

        _property = _serializedObject.FindProperty(_propertyPath);

        if (_property.managedReferenceValue == null)
        {
            var createButton = new Button
            {
                text = "Create New",
            };
            createButton.AddToClassList("create-button");
            createButton.clickable.clickedWithEventInfo += OnCreateNewButtonClicked;
            _root.Add(createButton);
        }
        else
        {
            string label = GetLabel(_property);
            var propertyField = new PropertyField(_property, label);
            propertyField.RegisterCallback<ContextualMenuPopulateEvent>(OnPopulatePropertyFieldContextualMenu);
            propertyField.TrackPropertyValue(_property, OnSerializedPropertyValueChanged);
            _root.Add(propertyField);
        }

        _root.Bind(_property.serializedObject);
    }

    private void OnSerializedPropertyValueChanged(SerializedProperty obj)
    {
        RebuildUI();
    }

    private void OnPopulatePropertyFieldContextualMenu(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction(
            "Delete",
            _ =>
            {
                _property.managedReferenceValue = null;
                _property.serializedObject.ApplyModifiedProperties();
            });
    }

    private void OnCreateNewButtonClicked(EventBase obj)
    {
        var target = (Button)obj.currentTarget;
        Rect rect = target.worldBound;
        rect.position = obj.originalMousePosition;
        var dropdown = new PolymorphicClassSelectorDropdown(((PolymorphicTypeSelectorAttribute)attribute).BaseType);
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

    // private void OnComponentPropertyFieldGeometryChanged(GeometryChangedEvent evt)
    // {
    //     var propertyField = (PropertyField)evt.target;
    //     if (propertyField.childCount == 0 || propertyField.ElementAt(0) is not Foldout originalFoldout)
    //     {
    //         return;
    //     }
    //
    //     SerializedProperty serializedProperty = _serializedObject.FindProperty(propertyField.bindingPath);
    //     VisualElement parent = propertyField.parent;
    //     int index = parent.IndexOf(propertyField);
    //
    //     var newContainer = new VisualElement();
    //     newContainer.AddToClassList("datacomponent-container");
    //
    //     var newFoldout = new Foldout
    //     {
    //         text = originalFoldout.text,
    //         value = originalFoldout.value,
    //         userData = propertyField.bindingPath,
    //     };
    //     newFoldout.AddToClassList("datacomponent-foldout");
    //     newFoldout.RegisterCallback<ContextClickEvent>(OnDataComponentFoldoutContextClicked);
    //     newFoldout.RegisterValueChangedCallback(evt =>
    //     {
    //         serializedProperty.isExpanded = evt.newValue;
    //         _serializedObject.ApplyModifiedProperties();
    //     });
    //     newFoldout.Add(new VisualElement().WithClass("datacomponent-border-light"));
    //     VisualElement foldoutContentContainer = originalFoldout.contentContainer;
    //     while (foldoutContentContainer.childCount > 0)
    //     {
    //         VisualElement child = foldoutContentContainer[0];
    //         newFoldout.Add(child);
    //     }
    //
    //     newFoldout.Add(new VisualElement().WithClass("datacomponent-border-dark"));
    //     newContainer.Add(newFoldout);
    //     parent.Insert(index, newContainer);
    //     propertyField.RemoveFromHierarchy();
    // }
}