// #undef UNITY_EDITOR

using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    /// <summary>
    /// A <see cref="ScriptableObject"/> that defines a bootstrap environment: its service list and the
    /// pre/post bootstrap handlers to run.
    /// </summary>
    [CreateAssetMenu(menuName = "Bootstrap/Environment Asset")]
    public class BootstrapEnvironmentAsset : ScriptableObject
    {
        /// <summary>
        /// True if this environment has no <see cref="ServiceListAsset"/> or the asset defines no services.
        /// </summary>
        public bool IsNoOp => ServiceListAsset.IsNull() || ServiceListAsset.Services.Length == 0;

        /// <summary>
        /// The handler to run before bootstrapping, or null to use the platform default.
        /// </summary>
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPreBootstrapHandler))]
        [field: Tooltip("The handler to run before bootstrapping, or null to use the platform default.")]
        public IPreBootstrapHandler PreBootstrapHandler { get; private set; }

        /// <summary>
        /// The handler to run after bootstrapping, or null to use the platform default.
        /// </summary>
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IPostBootstrapHandler))]
        [field: Tooltip("The handler to run after bootstrapping, or null to use the platform default.")]
        public IPostBootstrapHandler PostBootstrapHandler { get; private set; }

        /// <summary>
        /// The <see cref="ServiceList"/> this environment bootstraps.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The ServiceList this environment bootstraps.")]
        public ServiceList ServiceListAsset { get; private set; }

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