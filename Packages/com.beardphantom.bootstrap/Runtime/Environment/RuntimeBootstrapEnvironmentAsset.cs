using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Runtime Environment")]
    [SuppressMessage("ReSharper", "PartialTypeWithSinglePart")]
    public partial class RuntimeBootstrapEnvironmentAsset : BootstrapEnvironmentAsset
    {
        [field: SerializeField]
        [field: DisallowSceneObjects]
        private GameObject BootstrapperPrefab { get; set; }

        internal override GameObject StartEnvironment()
        {
            GameObject instance = Instantiate(BootstrapperPrefab);
            instance.name = $"[{name}] Bootstrapper";
            DontDestroyOnLoad(instance);
            return instance;
        }
    }
}