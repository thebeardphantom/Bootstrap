using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        public static event Action Initialized;
        public static event Action Deinitialized;

        private static AppInstance s_instance;

        public static AppInstance Instance
        {
            get
            {
                if (s_instance.IsNull())
                {
                    throw new InvalidOperationException("AppInstance is null.");
                }

                return s_instance;
            }
        }

        public static bool TryGetInstance(out AppInstance instance)
        {
            instance = s_instance;
            return instance.IsNotNull();
        }

        public static bool TryGetInstance<T>(out T instance) where T : AppInstance
        {
            if (TryGetInstance(out AppInstance untypedInstance) && untypedInstance is T typedInstance)
            {
                instance = typedInstance;
                return true;
            }

            instance = null;
            return false;
        }

        public static bool TryLocate<T>(out T service) where T : class
        {
            if (TryGetInstance(out AppInstance appInstance))
            {
                return appInstance.TryLocate(out service);
            }

            service = null;
            return false;
        }

        public static T Locate<T>() where T : class
        {
            return Instance.Locate<T>();
        }

        public static void Reset()
        {
            if (!TryGetInstance(out AppInstance instance))
            {
                return;
            }

            Logging.Info($"Deinitializing instance {s_instance}.");
            instance.NotifyResetting();
            s_instance = null;
            Deinitialized?.Invoke();
            SceneManager.LoadScene(0);
            RuntimeEntryPoint();
        }

        internal static void Deinitialize()
        {
            if (!TryGetInstance(out AppInstance instance))
            {
                return;
            }

            Logging.Info($"Deinitializing instance {s_instance}.");
            instance.NotifyQuitting();
            s_instance = null;
            Deinitialized?.Invoke();
        }

        internal static void Initialize<T>() where T : AppInstance, new()
        {
            InitializeAsync<T>().Forget();
        }

        internal static void Initialize(AppInstance instance)
        {
            InitializeAsync(instance).Forget();
        }

        private static Awaitable InitializeAsync<T>() where T : AppInstance, new()
        {
            ThrowIfInitialized();
            return InitializeAsync(new T());
        }

        private static async Awaitable InitializeAsync(AppInstance instance)
        {
            ThrowIfInitialized();
            Logging.Info($"Initializing instance {instance}.");
            s_instance = instance;
            await instance.BootstrapAsync();
            Initialized?.Invoke();
        }

        private static void ThrowIfInitialized()
        {
            if (s_instance != null)
            {
                throw new InvalidOperationException($"App already initialized with instance {s_instance}.");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeEntryPoint()
        {
            RuntimeAppInstance appInstance = new PlayModeAppInstance();
            InitializeAsync(appInstance).Forget();
        }
    }
}