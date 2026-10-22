#if UNITY_6000_3_OR_NEWER
using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Editor;
using System.Collections.Generic;
using UnityEditor.Toolbars;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Registers the Bootstrap dropdown menu (flow toggle, status window, app restart) on the Unity main toolbar.
/// </summary>
public static class BootstrapMainToolbarElements
{
    [MainToolbarElement("Bootstrap/Options", defaultDockPosition = MainToolbarDockPosition.Middle)]
    private static IEnumerable<MainToolbarElement> RegisterOptions()
    {
        yield return new MainToolbarDropdown(
            new MainToolbarContent("🥾Bootstrap"),
            rect =>
            {
                var menu = new GenericMenu();
                var isEditorFlowEnabled = BootstrapEditorSettingsUtility.GetValue(s => s.EditorFlowEnabled);
                menu.AddItem(new GUIContent("Flow Enabled"), isEditorFlowEnabled, ToggleFlowEnabled);

                menu.AddSeparator("");

                menu.AddItem(
                    new GUIContent("Status Window"),
                    false,
                    BootstrapAppWindow.ShowWindow);

                menu.AddItem(
                    new GUIContent("Restart App"),
                    false,
                    () =>
                    {
                        const string DecisionStorageKey = "com.beardphantom.bootstrap_AppResetOptOut";
                        const string Message = "This will discard active state and restart the bootstrapping process. "
                                               + "Do you want to continue to reset?";
                        const DialogOptOutDecisionType DecisionType = DialogOptOutDecisionType.ForThisSession;
                        var wantsReset = EditorUtility.DisplayDialog(
                            "Reset App?",
                            Message,
                            "Yes",
                            "No",
                            DecisionType,
                            DecisionStorageKey);
                        if (wantsReset)
                        {
                            App.Reset();
                        }
                    });

                menu.DropDown(rect);
            });
    }

    private static void ToggleFlowEnabled()
    {
        var isEditorFlowEnabled = BootstrapEditorSettingsUtility.GetValue(s => s.EditorFlowEnabled);
        var settingsAsset = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.User);
        var editorFlowEnabled = settingsAsset.EditorFlowEnabled;
        if (!editorFlowEnabled.OverrideEnabled)
        {
            editorFlowEnabled.OverrideEnabled = true;
        }

        editorFlowEnabled.SetValue(!isEditorFlowEnabled);
        settingsAsset.Save();
    }
}
#endif