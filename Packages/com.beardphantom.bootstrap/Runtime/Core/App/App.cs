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
                Logging.Warn("No current AppInstance.");
                return;
            }

            instance.NotifyResetting();
            Deinitialize();

            Logging.Debug("Reloading scene at index 0.");
            SceneManager.LoadScene(0);

            Logging.Debug($"Re-entering {nameof(RuntimeEntryPoint)}.");
            RuntimeEntryPoint();
        }

        internal static void Quit()
        {
            if (!TryGetInstance(out AppInstance instance))
            {
                Logging.Warn("No current AppInstance.");
                return;
            }

            instance.NotifyQuitting();
        }

        internal static void Initialize<T>() where T : AppInstance, new()
        {
            InitializeAsync<T>().Forget();
        }

        private static void Deinitialize()
        {
            Logging.Debug($"Deinitializing instance {s_instance}.");
            s_instance = null;
            Deinitialized?.Invoke();
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
            RuntimeAppInstance appInstance = GetRuntimeAppInstance();
            InitializeAsync(appInstance).Forget();
        }

        private static RuntimeAppInstance GetRuntimeAppInstance()
        {
#if UNITY_EDITOR
            return new PlayModeAppInstance();
#else
            return new BuildAppInstance();
#endif
        }
    }
}