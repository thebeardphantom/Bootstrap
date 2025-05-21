// #undef UNITY_EDITOR

#if !UNITY_EDITOR
using System;
#endif
using UnityEngine;

namespace BeardPhantom.Bootstrap.Environment
{
    [CreateAssetMenu(menuName = "Bootstrap/Runtime Environment")]
    public class BootstrapEnvironmentAsset : ScriptableObject
    {
        public static BootstrapEnvironmentAsset Instance { get; private set; }

        public bool IsNoOp => !BootstrapperPrefab;

        [field: SerializeField]
        [field: DisallowSceneObjects]
        internal GameObject BootstrapperPrefab { get; set; }

        internal GameObject StartEnvironment()
        {
            GameObject instance = Instantiate(BootstrapperPrefab);
            instance.name = $"[{name}] Bootstrapper";
            DontDestroyOnLoad(instance);
            return instance;
        }

#if !UNITY_EDITOR
        private void OnEnable()
        {
            if (Instance)
            {
                throw new InvalidOperationException($"{this} cannot replace existing instance {Instance}");
            }

            Instance = this;
        }
#endif
    }
}