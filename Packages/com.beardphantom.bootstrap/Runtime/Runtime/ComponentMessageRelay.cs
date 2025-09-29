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
            Logging.Debug("OnApplicationQuit");
            if (App.TryGetInstance(out AppInstance appInstance))
            {
                appInstance.NotifyQuitting();
            }

            DestroyImmediate(gameObject);
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void Update()
        {
            Logging.Trace("Update!");
            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                return;
            }

            if (!appInstance.CanLocateServices)
            {
                return;
            }

            float deltaTime = Time.deltaTime;
            foreach (IService service in appInstance.ServiceLocator)
            {
                if (service is IServiceWithUpdateLoop serviceWithUpdateLoop)
                {
                    serviceWithUpdateLoop.Update(deltaTime);
                }
            }
        }

        private void FixedUpdate()
        {
            Logging.Trace("FixedUpdate!");
            if (!App.TryGetInstance(out AppInstance appInstance))
            {
                return;
            }

            if (!appInstance.CanLocateServices)
            {
                return;
            }

            float fixedDeltaTime = Time.fixedDeltaTime;
            foreach (IService service in appInstance.ServiceLocator)
            {
                if (service is IServiceWithFixedUpdateLoop serviceWithFixedUpdateLoop)
                {
                    serviceWithFixedUpdateLoop.FixedUpdate(fixedDeltaTime);
                }
            }
        }

        private void OnRenderObject()
        {
            Logging.Trace("OnRenderObject!");
            // TODO: Add new interface for this. Replace boilerplate in this class with pre-filtered services in ServiceLocator.
        }
    }
}