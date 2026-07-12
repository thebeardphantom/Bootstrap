using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [CreateAssetMenu(menuName = "Bootstrap/" + nameof(ServiceList))]
    public class ServiceList : ScriptableObject
    {
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IService))]
        public IService[] Services { get; private set; } = Array.Empty<IService>();

        /// <summary>
        /// If this is a cloned instance of another asset then this property points to the source asset on disk.
        /// </summary>
        internal ServiceList Source { get; set; }

        public ServiceList CreateClone()
        {
            ServiceList clone = Instantiate(this);
            clone.Source = this;
            clone.name = $"{name} Instance";
            return clone;
        }
    }
}