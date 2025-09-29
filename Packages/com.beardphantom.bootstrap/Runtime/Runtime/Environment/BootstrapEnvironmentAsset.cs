// #undef UNITY_EDITOR

using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Environment Asset")]
    public class BootstrapEnvironmentAsset : ScriptableObject
    {
        public bool IsNoOp => ServiceListAsset == null || ServiceListAsset.Services.Length == 0;

        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPreBootstrapHandler))]
        public IPreBootstrapHandler PreBootstrapHandler { get; private set; }

        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPostBootstrapHandler))]
        public IPostBootstrapHandler PostBootstrapHandler { get; private set; }

        [field: SerializeField]
        public ServiceListAsset ServiceListAsset { get; private set; }

        internal BootstrapEnvironmentAsset StartEnvironment()
        {
#if UNITY_EDITOR
            BootstrapEnvironmentAsset copy = Instantiate(this);
            copy.ServiceListAsset = ServiceListAsset.CreateClone();
            return copy;
#else
            return this;
#endif
        }
    }
}