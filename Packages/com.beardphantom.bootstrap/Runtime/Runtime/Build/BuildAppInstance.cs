using BeardPhantom.Bootstrap.Environment;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public class BuildAppInstance : RuntimeAppInstance
    {
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
                if (asset != null)
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
    }
}