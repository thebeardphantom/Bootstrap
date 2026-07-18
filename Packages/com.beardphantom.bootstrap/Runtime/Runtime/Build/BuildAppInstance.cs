using BeardPhantom.Bootstrap.Environment;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// <see cref="RuntimeAppInstance"/> used for standalone builds. Determines the session environment from
    /// <see cref="BootstrapEnvironmentAsset"/> instances loaded in memory and defaults to
    /// <see cref="BuildBootstrapHandler"/> for bootstrap handling.
    /// </summary>
    public class BuildAppInstance : RuntimeAppInstance
    {
        /// <inheritdoc />
        protected override bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment)
        {
            BootstrapEnvironmentAsset[] bootstrapEnvironmentAssets = Resources.FindObjectsOfTypeAll<BootstrapEnvironmentAsset>();
            if (bootstrapEnvironmentAssets.Length == 0)
            {
                environment = null;
                return false;
            }

            environment = bootstrapEnvironmentAssets[0];
            var foundValid = false;
            foreach (BootstrapEnvironmentAsset asset in bootstrapEnvironmentAssets)
            {
                if (asset.IsNotNull())
                {
                    environment = asset;
                    foundValid = true;
                    break;
                }
            }

            if (foundValid && bootstrapEnvironmentAssets.Length > 1)
            {
                Logging.Warn($"Found multiple BootstrapEnvironmentAssets in memory. Using first valid environment '{environment}'.");
            }

            return foundValid;
        }

        /// <inheritdoc />
        protected override void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preBootstrapHandler,
            out IPostBootstrapHandler postBootstrapHandler)
        {
            preBootstrapHandler = BuildBootstrapHandler.Instance;
            postBootstrapHandler = BuildBootstrapHandler.Instance;
        }
    }
}