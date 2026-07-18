#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using BeardPhantom.Bootstrap.Environment;
using Newtonsoft.Json;
using UnityEditor;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// <see cref="RuntimeAppInstance"/> used when entering play mode in the editor. Determines the session
    /// environment from the <see cref="EditModeState"/> persisted before entering play mode and defaults to
    /// <see cref="PlayModeBootstrapHandler"/> for bootstrap handling.
    /// </summary>
    public class PlayModeAppInstance : RuntimeAppInstance
    {
        internal static bool TryLoadEditModeState(out EditModeState editModeState)
        {
            string json = SessionState.GetString(EditModeAppInstance.EditModeStateSessionStateKey, null);
            if (string.IsNullOrWhiteSpace(json))
            {
                editModeState = null;
                return false;
            }

            editModeState = JsonConvert.DeserializeObject<EditModeState>(json);
            return editModeState.IsNotNull();
        }

        /// <inheritdoc />
        protected override bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment)
        {
            if (!TryLoadEditModeState(out EditModeState editModeState))
            {
                environment = null;
                return false;
            }

            environment = editModeState.Environment;
            return editModeState.Environment;
        }

        /// <inheritdoc />
        protected override void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preBootstrapHandler,
            out IPostBootstrapHandler postBootstrapHandler)
        {
            preBootstrapHandler = PlayModeBootstrapHandler.Instance;
            postBootstrapHandler = PlayModeBootstrapHandler.Instance;
        }
    }
}
#endif