using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Static entry point for accessing and managing the current <see cref="AppInstance"/>.
    /// </summary>
    public static partial class App
    {
        /// <summary>
        /// Raised after the current <see cref="AppInstance"/> finishes bootstrapping.
        /// </summary>
        public static event Action Initialized;

        /// <summary>
        /// Raised after the current <see cref="AppInstance"/> is deinitialized.
        /// </summary>
        public static event Action Deinitialized;

        private static readonly Dictionary<Type, IAppExtension> s_typeToAppExtensions = new();

        private static int s_mainThreadId;

        private static AppInstance s_instance;

        /// <summary>
        /// True if the calling thread is the main thread the app was initialized on.
        /// </summary>
        public static bool IsMainThread => Thread.CurrentThread.ManagedThreadId == s_mainThreadId;

        /// <summary>
        /// The current app instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">No app instance is currently initialized.</exception>
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

        /// <summary>
        /// Instantiates a clone of <paramref name="original"/> and scopes it to the current app instance.
        /// </summary>
        /// <typeparam name="T">The type of object to instantiate.</typeparam>
        /// <param name="original">The object to clone.</param>
        public static T InstantiateAndScopeToApp<T>(T original) where T : Object
        {
            T clone = Object.Instantiate(original);
            return ScopeToApp(clone);
        }

        /// <summary>
        /// Scopes <paramref name="obj"/> to the current app instance, if one exists. See
        /// <see cref="AppInstance.ScopeToApp{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="obj"/>.</typeparam>
        /// <param name="obj">The object to scope to the current app instance.</param>
        /// <returns><paramref name="obj"/>, for chaining.</returns>
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

        /// <summary>
        /// Scopes each object in <paramref name="objs"/> to the current app instance, if one exists. See
        /// <see cref="AppInstance.ScopeToApp{T}"/>.
        /// </summary>
        /// <param name="objs">The objects to scope to the current app instance.</param>
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

        /// <summary>
        /// Attempts to get the current app instance.
        /// </summary>
        /// <param name="instance">The current app instance, or null if none is initialized.</param>
        /// <returns>True if an app instance is currently initialized.</returns>
        public static bool TryGetInstance(out AppInstance instance)
        {
            instance = s_instance;
            return instance.IsNotNull();
        }

        /// <summary>
        /// Attempts to get the current app instance as type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The expected app instance type.</typeparam>
        /// <param name="instance">The current app instance cast to <typeparamref name="T"/>, or null if none is
        /// initialized or the current instance is not of type <typeparamref name="T"/>.</param>
        /// <returns>True if an app instance of type <typeparamref name="T"/> is currently initialized.</returns>
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

        /// <summary>
        /// Attempts to locate a service of type <typeparamref name="T"/> via the current app instance.
        /// </summary>
        /// <typeparam name="T">The service type to locate.</typeparam>
        /// <param name="service">The located service, or null if not found or no app instance is initialized.</param>
        /// <returns>True if the service was located.</returns>
        public static bool TryLocate<T>(out T service) where T : class
        {
            if (TryGetInstance(out AppInstance appInstance))
            {
                return appInstance.TryLocate(out service);
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Locates a service of type <typeparamref name="T"/> via <see cref="Instance"/>.
        /// </summary>
        /// <typeparam name="T">The service type to locate.</typeparam>
        public static T Locate<T>() where T : class
        {
            return Instance.Locate<T>();
        }

        /// <summary>
        /// Gets the registered <see cref="IAppExtension"/> instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The extension type to get.</typeparam>
        /// <exception cref="Exception">No extension of type <typeparamref name="T"/> is registered.</exception>
        public static T GetExtension<T>() where T : IAppExtension
        {
            if (s_typeToAppExtensions.TryGetValue(typeof(T), out IAppExtension extension))
            {
                return (T)extension;
            }

            throw new Exception($"No extension found for type {typeof(T)}");
        }

        /// <summary>
        /// Resets the current app instance and reinitializes the app, reloading scene 0 when playing.
        /// </summary>
        public static void Reset()
        {
            if (!TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn("No current AppInstance.");
                return;
            }

            appInstance.NotifyResetting();
            Deinitialize();

            if (Application.isPlaying)
            {
                Logging.Debug("Reloading scene at index 0.");
                SceneManager.LoadScene(0);

                Logging.Debug($"Re-entering {nameof(RuntimeEntryPoint)}.");
                RuntimeEntryPoint();
            }
            else
            {
                ScheduleInitEditorApp();
            }
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

        private static void InitCommon()
        {
            s_mainThreadId = Thread.CurrentThread.ManagedThreadId;
            Logging.Trace($"Got main thread ID {s_mainThreadId}.");
            RegisterAllExtensions();
        }

        private static void RegisterAllExtensions()
        {
            if (s_typeToAppExtensions.Count > 0)
            {
                return;
            }

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

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeEntryPoint()
        {
            InitCommon();
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