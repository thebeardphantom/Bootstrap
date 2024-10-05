using BeardPhantom.Bootstrap.Editor.Settings;
using BeardPhantom.Bootstrap.Environment;
using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [InitializeOnLoad]
    public static class EnvironmentSelectorUI
    {
        private const string SceneOverrideLabel = "Use Scene Environment";

        private static EditorToolbarDropdown s_editorToolbarDropdown;

        static EnvironmentSelectorUI()
        {
            Type playmodeButtons = typeof(EditorToolbarToggle)
                .Assembly
                .GetType("UnityEditor.Toolbars.PlayModeButtons", true);
            EventInfo evt = playmodeButtons.GetEvent(
                "onPlayModeButtonsCreated",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            MethodInfo addMethod = evt.GetAddMethod(true);
            addMethod.Invoke(
                null,
                new object[]
                {
                    new Action<VisualElement>(OnPlayModeButtonsCreated),
                });
        }

        private static void OnPlayModeButtonsCreated(VisualElement visualElement)
        {
            visualElement.schedule.Execute(
                    () =>
                    {
                        CreateUI(visualElement);
                    })
                .ExecuteLater(100);
        }

        private static void CreateUI(VisualElement visualElement)
        {
            VisualElement root = visualElement.panel.visualTree.Q("ToolbarZoneRightAlign");
            s_editorToolbarDropdown = new EditorToolbarDropdown(OnOpenEnvironmentDropdown);
            UpdateUI();
            root.Add(s_editorToolbarDropdown);
        }

        private static void OnOpenEnvironmentDropdown()
        {
            var menu = new GenericMenu();
            BootstrapEditorUserSettings userSettings = BootstrapEditorUserSettings.instance;
            menu.AddItem(
                new GUIContent(SceneOverrideLabel),
                false,
                () =>
                {
                    userSettings.SelectedEnvironment = default;
                    UpdateUI();
                });
            menu.AddSeparator("");
            menu.AddItem(
                new GUIContent("Select Asset..."),
                false,
                () =>
                {
                    BootstrapEditorUtility.PickAsset<BootstrapEnvironmentAsset>(
                        result =>
                        {
                            userSettings.SelectedEnvironment = result;
                            UpdateUI();
                        });
                });
            menu.ShowAsContext();
        }

        private static void UpdateUI()
        {
            BootstrapEnvironmentAsset environment = BootstrapEditorUserSettings.instance.SelectedEnvironment;
            string label = environment == null
                ? SceneOverrideLabel
                : environment.name;
            label = "🥾 " + label;
            s_editorToolbarDropdown.text = label;
        }
    }
}