// #undef UNITY_EDITOR

using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Environment Asset")]
    public class BootstrapEnvironmentAsset : ScriptableObject
    {
        public bool IsNoOp => ServicesListAsset == null || ServicesListAsset.Services.Length == 0;

        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPreBootstrapHandler))]
        public IPreBootstrapHandler PreBootstrapHandler { get; private set; }

        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPostBootstrapHandler))]
        public IPostBootstrapHandler PostBootstrapHandler { get; private set; }

        [field: SerializeField]
        public ServicesListAsset ServicesListAsset { get; private set; }

        internal BootstrapEnvironmentAsset StartEnvironment()
        {
#if UNITY_EDITOR
            BootstrapEnvironmentAsset copy = Instantiate(this);
            copy.ServicesListAsset = Instantiate(ServicesListAsset);
            return copy;
#else
            return this;
#endif
        }
    }
}