using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-10000)]
    internal class ComponentMessageRelay : MonoBehaviour
    {
        private static ComponentMessageRelay s_instance;

        public static ComponentMessageRelay Instance
        {
            get =>
                s_instance.IsNull()
                    ? throw new InvalidOperationException($"{nameof(ComponentMessageRelay)} has not been initialized.")
                    : s_instance;
            private set => s_instance = value;
        }

        public static ComponentMessageRelay Create()
        {
            if (s_instance.IsNull())
            {
                var relayGameObject = new GameObject(nameof(ComponentMessageRelay));
                s_instance = relayGameObject.AddComponent<ComponentMessageRelay>();
                DontDestroyOnLoad(relayGameObject);
                relayGameObject.hideFlags = HideFlags.HideAndDontSave;
            }

            return Instance;
        }

        private void OnApplicationQuit()
        {
            App.Quit();
            DestroyImmediate(gameObject);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            if (!App.TryGetInstance(out var appInstance))
            {
                return;
            }

            if (!appInstance.CanLocateServices)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            var serviceLocator = appInstance.ServiceLocator;
            for (var i = 0; i < serviceLocator.Count; i++)
            {
                var service = serviceLocator[i];
                if (service is IServiceWithUpdateLoop serviceWithUpdateLoop)
                {
                    serviceWithUpdateLoop.Update(deltaTime);
                }
            }
        }

        private void LateUpdate()
        {
            if (!App.TryGetInstance(out var appInstance))
            {
                return;
            }

            if (!appInstance.CanLocateServices)
            {
                return;
            }

            var deltaTime = Time.deltaTime;
            var serviceLocator = appInstance.ServiceLocator;
            for (var i = 0; i < serviceLocator.Count; i++)
            {
                var service = serviceLocator[i];
                if (service is IServiceWithLateUpdateLoop serviceWithLateUpdateLoop)
                {
                    serviceWithLateUpdateLoop.LateUpdate(deltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!App.TryGetInstance(out var appInstance))
            {
                return;
            }

            if (!appInstance.CanLocateServices)
            {
                return;
            }

            var fixedDeltaTime = Time.fixedDeltaTime;
            var serviceLocator = appInstance.ServiceLocator;
            for (var i = 0; i < serviceLocator.Count; i++)
            {
                var service = serviceLocator[i];
                if (service is IServiceWithFixedUpdateLoop serviceWithFixedUpdateLoop)
                {
                    serviceWithFixedUpdateLoop.FixedUpdate(fixedDeltaTime);
                }
            }
        }
    }
}