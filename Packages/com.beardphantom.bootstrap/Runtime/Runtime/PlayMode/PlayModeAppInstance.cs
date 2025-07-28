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
            if (change is not PlayModeStateChange.EnteredEditMode)
            {
                return;
            }

            if (BootstrapEditorSettingsUtility.GetValue(a => a.EditorFlowEnabled))
            {
                EditorSceneManager.playModeStartScene = null;
            }

            App.InitializeEditorDelayed<EditModeAppInstance>();

            if (App.TryGetInstance<PlayModeAppInstance>(out _))
            {
                App.Deinitialize();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            EditorApplication.playModeStateChanged -= OnPlaymodeStateChanged;
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