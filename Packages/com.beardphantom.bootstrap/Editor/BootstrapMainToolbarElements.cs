#if UNITY_6000_3_OR_NEWER
using BeardPhantom.Bootstrap;
using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

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
                IBootstrapEditorSettingsAsset settingsAsset = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.User);
                SettingsProperty<bool> editorFlowEnabled = settingsAsset.EditorFlowEnabled;
                bool currentValue = editorFlowEnabled.OverrideEnabled && editorFlowEnabled.Value;
                menu.AddItem(new GUIContent("Flow Enabled"), currentValue, ToggleFlowEnabled);

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
                        const string DecisionStorageKey = "AppResetOptOut";
                        const string Message = "This will discard active state and restart the bootstrapping process. "
                                               + "Do you want to continue to reset?";
                        const DialogOptOutDecisionType DecisionType = DialogOptOutDecisionType.ForThisSession;
                        EditorUtility.GetDialogOptOutDecision(DecisionType, DecisionStorageKey);
                        bool wantsReset = EditorUtility.DisplayDialog(
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
        IBootstrapEditorSettingsAsset settingsAsset = BootstrapEditorSettingsUtility.GetWithScope(SettingsScope.User);
        SettingsProperty<bool> editorFlowEnabled = settingsAsset.EditorFlowEnabled;
        if (!editorFlowEnabled.OverrideEnabled)
        {
            editorFlowEnabled.OverrideEnabled = true;
        }

        editorFlowEnabled.SetValue(!editorFlowEnabled.Value);
    }
}
#endif