using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// A ScriptableObject asset containing the ordered set of services to be discovered and initialized during
    /// bootstrap.
    /// </summary>
    [CreateAssetMenu(menuName = "Bootstrap/" + nameof(ServiceList))]
    public class ServiceList : ScriptableObject
    {
        /// <summary>
        /// The services contained in this list.
        /// </summary>
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IService))]
        [field: Tooltip("The services contained in this list.")]
        public IService[] Services { get; private set; } = Array.Empty<IService>();

        /// <summary>
        /// If this is a cloned instance of another asset then this property points to the source asset on disk.
        /// </summary>
        internal ServiceList Source { get; set; }

        /// <summary>
        /// Creates a runtime clone of this asset, so services can be safely mutated without affecting the
        /// asset on disk.
        /// </summary>
        /// <returns>The cloned <see cref="ServiceList"/>, with <see cref="Source"/> set to this instance.</returns>
        public ServiceList CreateClone()
        {
            ServiceList clone = Instantiate(this);
            clone.Source = this;
            clone.name = $"{name} Instance";
            return clone;
        }
    }
}