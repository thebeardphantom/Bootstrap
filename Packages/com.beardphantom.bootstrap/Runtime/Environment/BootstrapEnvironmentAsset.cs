using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    public abstract class BootstrapEnvironmentAsset : ScriptableObject
    {
        internal abstract GameObject StartEnvironment();
    }
}