#if !UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        private static bool TryDetermineSessionEnvironmentInBuild(out RuntimeBootstrapEnvironmentAsset environment)
        {
            environment = RuntimeBootstrapEnvironmentAsset.Instance;
            return environment != null;
        }
    }
}
#endif