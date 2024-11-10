// #undef UNITY_EDITOR

using UnityEngine;
#if !UNITY_EDITOR
using System;
#endif

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Runtime Environment")]
    public partial class RuntimeBootstrapEnvironmentAsset : BootstrapEnvironmentAsset
    {
        public override bool IsNoOp => BootstrapperPrefab == null;

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

#if !UNITY_EDITOR
    public partial class RuntimeBootstrapEnvironmentAsset
    {
        public static RuntimeBootstrapEnvironmentAsset Instance { get; private set; }

        private void OnEnable()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"{this} cannot replace existing instance {Instance}");
            }

            Instance = this;
        }
    }
#endif
}