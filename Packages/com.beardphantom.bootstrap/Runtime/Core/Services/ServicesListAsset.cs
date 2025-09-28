using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [CreateAssetMenu(menuName = "Bootstrap/" + nameof(ServicesListAsset))]
    public class ServicesListAsset : ScriptableObject
    {
        [field: SerializeReference]
        [field: PolymorphicTypeSelector(typeof(IBootstrapService))]
        public IBootstrapService[] Services { get; private set; } = Array.Empty<IBootstrapService>();
    }
}