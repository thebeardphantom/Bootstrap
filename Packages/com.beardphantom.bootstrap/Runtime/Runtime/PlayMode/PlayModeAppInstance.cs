#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public class PlayModeAppInstance : RuntimeAppInstance
    {
        public PlayModeAppInstance()
        {
            EditorApplication.playModeStateChanged += OnPlaymodeStateChanged;
        }

        private static void OnPlaymodeStateChanged(PlayModeStateChange change)
        {
            switch (change)
            {
                case PlayModeStateChange.ExitingPlayMode:
                {
                    App.Dispose();
                    break;
                }
                case PlayModeStateChange.EnteredEditMode:
                {
                    EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
                    if (BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
                    {
                        EditorSceneManager.playModeStartScene = null;
                    }

                    App.Deinitialize();
                    App.InitializeEditorDelayed<EditModeAppInstance>();
                    break;
                }
            }
        }

        protected override bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment)
        {
            if (!BootstrapUtility.TryLoadEditModeState(out EditModeState editModeState))
            {
                environment = null;
                return false;
            }

            environment = editModeState.Environment;
            return editModeState.Environment;
        }
    }
}
#endif