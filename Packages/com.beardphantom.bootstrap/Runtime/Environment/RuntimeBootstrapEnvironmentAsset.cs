using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Runtime Environment")]
    public class RuntimeBootstrapEnvironmentAsset : BootstrapEnvironmentAsset
    {
        public static RuntimeBootstrapEnvironmentAsset Instance { get; private set; }

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

        private void OnEnable()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException($"{this} cannot replace existing instance {Instance}");
            }

            Instance = this;
        }
    }
}