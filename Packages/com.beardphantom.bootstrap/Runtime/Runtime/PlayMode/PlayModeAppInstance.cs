#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using UnityEditor;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// <see cref="RuntimeAppInstance" /> used when entering play mode in the editor. Determines the session
    /// environment from the <see cref="EditMode.EditModeState" /> persisted before entering play mode and defaults to
    /// <see cref="PlayModeBootstrapHandler" /> for bootstrap handling.
    /// </summary>
    public class PlayModeAppInstance : RuntimeAppInstance
    {
        /// <summary>
        /// The serialized state of the editor before entering playmode.
        /// </summary>
        public EditModeState EditModeState { get; private set; }

        private static bool TryLoadEditModeState(out EditModeState editModeState)
        {
            var json = SessionState.GetString(EditModeAppInstance.EditModeStateSessionStateKey, null);
            if (string.IsNullOrWhiteSpace(json))
            {
                editModeState = null;
                return false;
            }

            editModeState = JsonConvert.DeserializeObject<EditModeState>(json);
            return editModeState.IsNotNull();
        }

        /// <inheritdoc />
        public override void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preBootstrapHandler,
            out IPostBootstrapHandler postBootstrapHandler)
        {
            preBootstrapHandler = PlayModeBootstrapHandler.Instance;
            postBootstrapHandler = PlayModeBootstrapHandler.Instance;
        }

        /// <inheritdoc />
        protected override bool TryLoadEnvironment(out BootstrapEnvironmentAsset environment)
        {
            if (!TryLoadEditModeState(out var editModeState) || editModeState.Environment.IsNull())
            {
                environment = null;
                EditModeState = null;
                return false;
            }

            App.State.IsRunningTests |= editModeState.HasTestRunnerScene;
            environment = editModeState.Environment;
            EditModeState = editModeState;
            return true;
        }
    }
}
#endif