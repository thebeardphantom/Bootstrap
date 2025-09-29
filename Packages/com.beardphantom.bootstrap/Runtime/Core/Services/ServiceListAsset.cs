using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [CreateAssetMenu(menuName = "Bootstrap/" + nameof(ServiceListAsset))]
    public class ServiceListAsset : ScriptableObject
    {
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IService))]
        public IService[] Services { get; private set; } = Array.Empty<IService>();

        /// <summary>
        /// If this is a cloned instance of another asset then this property points to the source asset on disk.
        /// </summary>
        internal ServiceListAsset SourceAsset { get; set; }

        public ServiceListAsset CreateClone()
        {
            ServiceListAsset clone = Instantiate(this);
            clone.SourceAsset = this;
            return clone;
        }
    }
}