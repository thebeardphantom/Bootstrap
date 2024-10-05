using BeardPhantom.Bootstrap.Environment;
using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [Serializable]
    public class MappedEnvironment<T>
    {
        [field: SerializeField]
        public T Key { get; set; }

        [field: SerializeField]
        public RuntimeBootstrapEnvironmentAsset Environment { get; set; }
    }
}