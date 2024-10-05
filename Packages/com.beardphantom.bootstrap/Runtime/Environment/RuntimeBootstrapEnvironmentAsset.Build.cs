#if !UNITY_EDITOR
using System;

namespace BeardPhantom.Bootstrap.Environment
{
    public partial class RuntimeBootstrapEnvironmentAsset
    {
        public static RuntimeBootstrapEnvironmentAsset Instance { get; private set; }

        private void OnEnable()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"{this} cannot replace existing instance {Instance}");
            }

            Instance = this;
        }
    }
}
#endif