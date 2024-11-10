using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    public abstract class BootstrapEnvironmentAsset : ScriptableObject
    {
        public abstract bool IsNoOp { get; }

        internal abstract GameObject StartEnvironment();
    }
}