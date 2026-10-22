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

        public static void Create()
        {
            if (s_instance.IsNotNull())
            {
                return;
            }

            var relayGameObject = new GameObject(nameof(ComponentMessageRelay));
            s_instance = relayGameObject.AddComponent<ComponentMessageRelay>();
            DontDestroyOnLoad(relayGameObject);
            relayGameObject.hideFlags = HideFlags.HideAndDontSave;
        }

        private static bool TryGetServiceLocator(out ServiceLocator serviceLocator)
        {
            serviceLocator = null;
            if (!App.TryGetInstance(out var instance))
            {
                return false;
            }

            if (!instance.CanLocateServices)
            {
                return false;
            }

            serviceLocator = instance.ServiceLocator;
            return true;
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

        private void FixedUpdate()
        {
            if (!TryGetServiceLocator(out var serviceLocator))
            {
                return;
            }

            var servicesWithFixedUpdateLoop = serviceLocator.ServicesWithFixedUpdateLoop;
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < servicesWithFixedUpdateLoop.Count; i++)
            {
                var service = servicesWithFixedUpdateLoop[i];
                service.FixedUpdate(deltaTime);
            }
        }

        private void Update()
        {
            if (!TryGetServiceLocator(out var serviceLocator))
            {
                return;
            }

            var servicesWithUpdateLoop = serviceLocator.ServicesWithUpdateLoop;
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < servicesWithUpdateLoop.Count; i++)
            {
                var service = servicesWithUpdateLoop[i];
                service.Update(deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (!TryGetServiceLocator(out var serviceLocator))
            {
                return;
            }

            var servicesWithLateUpdateLoop = serviceLocator.ServicesWithLateUpdateLoop;
            var deltaTime = Time.deltaTime;
            for (var i = 0; i < servicesWithLateUpdateLoop.Count; i++)
            {
                var service = servicesWithLateUpdateLoop[i];
                service.LateUpdate(deltaTime);
            }
        }
    }
}