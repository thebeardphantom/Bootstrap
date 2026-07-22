using BeardPhantom.Bootstrap.EditMode;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.SceneManagement;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Static entry point for accessing and managing the current <see cref="AppInstance" />.
    /// </summary>
    public static partial class App
    {
        /// <summary>
        /// Raised after the current <see cref="AppInstance" /> finishes bootstrapping.
        /// </summary>
        public static event Action AppInstanceCreated;

        /// <summary>
        /// Raised after the current <see cref="AppInstance" /> is deinitialized.
        /// </summary>
        public static event Action AppInstanceDestroyed;

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
                    throw new InvalidOperationException($"{nameof(AppInstance)} is null.");
                }

                return s_instance;
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
        /// Attempts to get the current app instance as type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The expected app instance type.</typeparam>
        /// <param name="instance">
        /// The current app instance cast to <typeparamref name="T" />, or null if none is
        /// initialized or the current instance is not of type <typeparamref name="T" />.
        /// </param>
        /// <returns>True if an app instance of type <typeparamref name="T" /> is currently initialized.</returns>
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
        /// Attempts to locate a service of type <typeparamref name="T" /> via the current app instance.
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
        /// Locates a service of type <typeparamref name="T" /> via <see cref="Instance" />.
        /// </summary>
        /// <typeparam name="T">The service type to locate.</typeparam>
        public static T Locate<T>() where T : class
        {
            return Instance.Locate<T>();
        }

        /// <summary>
        /// Gets the registered <see cref="IAppExtension" /> instance of type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The extension type to get.</typeparam>
        /// <exception cref="Exception">No extension of type <typeparamref name="T" /> is registered.</exception>
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
                Logging.Warn($"No current {nameof(AppInstance)}.");
                return;
            }

            appInstance.NotifyResetting();
            DestroyAppInstance();

            if (Application.isPlaying)
            {
                Logging.Debug("Reloading scene at index 0.");
                SceneManager.LoadScene(0);

                Logging.Debug($"Re-entering {nameof(RuntimeEntryPoint)}.");
                RuntimeEntryPoint();
            }
            else
            {
                CreateAppInstanceDelayed<EditModeAppInstance>();
            }
        }

        internal static void Quit()
        {
            if (!TryGetInstance(out AppInstance appInstance))
            {
                Logging.Warn($"No current {nameof(AppInstance)}.");
                return;
            }

            appInstance.NotifyQuitting();
        }

        internal static void CreateAppInstance<T>() where T : AppInstance, new()
        {
            CreateAppInstanceAsync<T>().Forget();
        }

        internal static void DestroyAppInstance()
        {
            Logging.Debug($"Destroying {nameof(AppInstance)} {s_instance}.");
            s_instance = null;
            AppInstanceDestroyed?.Invoke();
        }

        private static Awaitable CreateAppInstanceAsync<T>() where T : AppInstance, new()
        {
            ThrowIfAppInstanceCreated();
            return CreateAppInstanceAsync(new T());
        }

        private static async Awaitable CreateAppInstanceAsync(AppInstance appInstance)
        {
            ThrowIfAppInstanceCreated();
            Logging.Info($"Creating {nameof(AppInstance)} {appInstance}.");
            s_instance = appInstance;
            await appInstance.BootstrapAsync();
            AppInstanceCreated?.Invoke();
        }

        private static void ThrowIfAppInstanceCreated()
        {
            if (s_instance.IsNotNull())
            {
                throw new InvalidOperationException($"{nameof(AppInstance)} already exists: {s_instance}.");
            }
        }

        private static void Init()
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
            Init();
            RuntimeAppInstance appInstance = GetRuntimeAppInstance();
            CreateAppInstanceAsync(appInstance).Forget();
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