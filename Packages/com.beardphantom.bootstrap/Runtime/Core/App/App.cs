using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        public static event Action BootstrappingComplete;

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

        public static void Deinitialize()
        {
            if (!TryGetInstance(out AppInstance _))
            {
                return;
            }

            Logging.Info($"Deinitializing instance {s_instance}.");
            s_instance.Dispose();
            s_instance = null;
        }

        internal static void Initialize<T>() where T : AppInstance, new()
        {
            InitializeAsync<T>().Forget();
        }

        internal static void Initialize(AppInstance instance)
        {
            InitializeAsync(instance).Forget();
        }

        internal static Awaitable InitializeAsync<T>() where T : AppInstance, new()
        {
            ThrowIfInitialized();

            return InitializeAsync(new T());
        }

        internal static async Awaitable InitializeAsync(AppInstance instance)
        {
            ThrowIfInitialized();

            Logging.Info($"Initializing instance {instance}.");
            s_instance = instance;
            await instance.BootstrapAsync();
            BootstrappingComplete?.Invoke();
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
            RuntimeAppInstance appInstance = BootstrapUtility.GetRuntimeAppInstance();
            InitializeAsync(appInstance).Forget();
        }
    }
}