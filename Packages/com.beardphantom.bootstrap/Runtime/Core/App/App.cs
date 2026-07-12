using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        public static event Action Initialized;

        public static event Action Deinitialized;

        private static readonly Dictionary<Type, IAppExtension> s_typeToAppExtensions = new();

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

        public static T InstantiateAndScopeToApp<T>(T original) where T : Object
        {
            T clone = Object.Instantiate(original);
            return ScopeToApp(clone);
        }

        public static T ScopeToApp<T>(T obj) where T : Object
        {
            if (TryGetInstance(out AppInstance appInstance))
            {
                appInstance.ScopeToApp(obj);
            }
            else
            {
                Logging.Warn("No current AppInstance.");
            }

            return obj;
        }

        public static void ScopeToApp(IEnumerable<Object> objs)
        {
            if (!TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn("No current AppInstance.");
                return;
            }

            foreach (Object obj in objs)
            {
                appInstance.ScopeToApp(obj);
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
            if (!TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn("No current AppInstance.");
                return;
            }

            appInstance.NotifyResetting();
            Deinitialize();

            Logging.Debug("Reloading scene at index 0.");
            SceneManager.LoadScene(0);

            Logging.Debug($"Re-entering {nameof(RuntimeEntryPoint)}.");
            RuntimeEntryPoint();
        }

        public static T GetExtension<T>() where T : IAppExtension
        {
            if (s_typeToAppExtensions.TryGetValue(typeof(T), out IAppExtension extension))
            {
                return (T)extension;
            }

            throw new Exception($"No extension found for type {typeof(T)}");
        }

        internal static void Quit()
        {
            if (!TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn("No current AppInstance.");
                return;
            }

            appInstance.NotifyQuitting();
        }

        internal static void Initialize<T>() where T : AppInstance, new()
        {
            InitializeAsync<T>().Forget();
        }

        internal static void Deinitialize()
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

        private static async Awaitable InitializeAsync(AppInstance appInstance)
        {
            ThrowIfInitialized();
            Logging.Info($"Initializing instance {appInstance}.");
            s_instance = appInstance;
            await appInstance.BootstrapAsync();
            Initialized?.Invoke();
        }

        private static void ThrowIfInitialized()
        {
            if (s_instance.IsNotNull())
            {
                throw new InvalidOperationException($"App already initialized with instance {s_instance}.");
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeEntryPoint()
        {
#if !UNITY_EDITOR
            RegisterAllExtensions();
#endif
            RuntimeAppInstance appInstance = GetRuntimeAppInstance();
            InitializeAsync(appInstance).Forget();
        }

        private static void RegisterAllExtensions()
        {
            IReadOnlyList<Assembly> assemblies = CurrentAssemblies.GetLoadedAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                IEnumerable<AppExtensionTypeAttribute> attributes = assembly.GetCustomAttributes<AppExtensionTypeAttribute>();
                foreach (AppExtensionTypeAttribute attribute in attributes)
                {
                    var appExtensionInstance = (IAppExtension)Activator.CreateInstance(attribute.ExtensionType);
                    s_typeToAppExtensions.Add(attribute.ExtensionType, appExtensionInstance);
                    Logging.Debug($"Registered extension {attribute.ExtensionType.Name}.");
                }
            }
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