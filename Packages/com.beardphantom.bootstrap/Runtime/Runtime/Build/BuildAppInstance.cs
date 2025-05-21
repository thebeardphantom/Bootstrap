using BeardPhantom.Bootstrap.Environment;

namespace BeardPhantom.Bootstrap
{
    public class BuildAppInstance : RuntimeAppInstance
    {
        protected override bool TryDetermineSessionEnvironment(out BootstrapEnvironmentAsset environment)
        {
            environment = BootstrapEnvironmentAsset.Instance;
            return environment;
        }
    }
}