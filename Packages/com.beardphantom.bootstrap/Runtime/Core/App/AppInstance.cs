using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Base class for an instance of the running app, owning its <see cref="ServiceLocator" />,
    /// <see cref="TaskScheduler" />, and bootstrap lifecycle.
    /// </summary>
    public abstract class AppInstance : IDisposable
    {
        /// <summary>
        /// Invoked when <see cref="BootstrapState" /> changes.
        /// </summary>
        /// <param name="previousState">The state being transitioned away from.</param>
        /// <param name="newState">The state being transitioned to.</param>
        public delegate void OnAppBootstrapStateChanged(in AppBootstrapState previousState, in AppBootstrapState newState);

        /// <summary>
        /// Raised whenever <see cref="BootstrapState" /> changes.
        /// </summary>
        public event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        private readonly CancellationTokenSource _appLifetimeCancellationTokenSource = new();

        private readonly HashSet<Object> _appScopedObjects = new();

        private AppBootstrapState _bootstrapState;

        /// <summary>
        /// Cancelled when this app instance is disposed, quitting, or resetting.
        /// </summary>
        public CancellationToken AppLifetimeCancellationToken => _appLifetimeCancellationTokenSource.Token;

        /// <summary>
        /// The set of services active for this app instance.
        /// </summary>
        public abstract ServiceList ActiveServiceList { get; }

        /// <summary>
        /// The current phase of the bootstrap lifecycle. Setting this raises <see cref="AppBootstrapStateChanged" />.
        /// </summary>
        public AppBootstrapState BootstrapState
        {
            get => _bootstrapState;
            internal set
            {
                if (_bootstrapState == value)
                {
                    return;
                }

                AppBootstrapState previousState = _bootstrapState;
                _bootstrapState = value;
                AppBootstrapStateChanged?.Invoke(previousState, value);
            }
        }

        /// <summary>
        /// The service locator used to resolve services for this app instance.
        /// </summary>
        public ServiceLocator ServiceLocator { get; private set; }

        /// <summary>
        /// The time this app instance was created.
        /// </summary>
        public DateTimeOffset CreateTimestamp { get; private set; }

        /// <summary>
        /// A unique identifier generated for this app instance's session.
        /// </summary>
        public Guid SessionGuid { get; private set; }

        /// <summary>
        /// True if the app instance is currently running (e.g. in play mode or a build).
        /// </summary>
        public bool IsPlaying { get; protected set; }

        /// <summary>
        /// True if this app instance is running under a test runner.
        /// </summary>
        public bool IsRunningTests { get; internal set; }

        /// <summary>
        /// True if <see cref="BootstrapState" /> is <see cref="AppBootstrapState.Quitting" />.
        /// </summary>
        public virtual bool IsQuitting => BootstrapState == AppBootstrapState.Quitting;

        /// <summary>
        /// True if <see cref="BootstrapState" /> is <see cref="AppBootstrapState.Resetting" />.
        /// </summary>
        public virtual bool IsResetting => BootstrapState == AppBootstrapState.Resetting;

        /// <summary>
        /// The task scheduler associated with this app instance.
        /// </summary>
        public TaskScheduler TaskScheduler { get; private set; }

        /// <summary>
        /// True if <see cref="ServiceLocator" /> is available and able to locate services.
        /// </summary>
        public bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        /// <summary>
        /// True if this app instance has already been disposed.
        /// </summary>
        protected bool Disposed { get; private set; }

        /// <summary>
        /// Disposes this app instance, cancelling <see cref="AppLifetimeCancellationToken" /> and disposing
        /// <see cref="ServiceLocator" />. Safe to call multiple times.
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Logging.Debug($"Disposing {this}.");
            Disposed = true;
            _appLifetimeCancellationTokenSource.Cancel();
            _appLifetimeCancellationTokenSource.Dispose();
            ServiceLocator.Dispose();
        }

        /// <summary>
        /// Attempts to locate a service of type <typeparamref name="T" /> via <see cref="ServiceLocator" />.
        /// </summary>
        /// <typeparam name="T">The service type to locate.</typeparam>
        /// <param name="service">The located service, or null if not found.</param>
        /// <returns>True if the service was located.</returns>
        public bool TryLocate<T>(out T service) where T : class
        {
            return ServiceLocator.TryLocateService(out service);
        }

        /// <summary>
        /// Locates a service of type <typeparamref name="T" /> via <see cref="ServiceLocator" />.
        /// </summary>
        /// <typeparam name="T">The service type to locate.</typeparam>
        public T Locate<T>() where T : class
        {
            return ServiceLocator.LocateService<T>();
        }

        /// <summary>
        /// Marks <paramref name="obj" /> as scoped to this app instance: it is preserved across scene loads via
        /// <see cref="Object.DontDestroyOnLoad" /> and destroyed when this app instance resets.
        /// </summary>
        /// <typeparam name="T">The type of <paramref name="obj" />.</typeparam>
        /// <param name="obj">The object to scope to this app instance.</param>
        /// <returns><paramref name="obj" />, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="obj" /> is null.</exception>
        public T ScopeToApp<T>(T obj) where T : Object
        {
            if (obj.IsNull())
            {
                throw new ArgumentNullException(nameof(obj));
            }

            Object.DontDestroyOnLoad(obj);
            _appScopedObjects.Add(obj);
            return obj;
        }

        /// <summary>
        /// Instantiates a clone of <paramref name="original" /> and scopes it to the current app instance.
        /// See <see cref="ScopeToApp{T}" />.
        /// </summary>
        /// <typeparam name="T">The type of object to instantiate.</typeparam>
        /// <param name="original">The object to clone.</param>
        /// <returns>The cloned instance of <paramref name="original" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="original" /> is null.</exception>
        public T InstantiateAndScopeToApp<T>(T original) where T : Object
        {
            if (original.IsNull())
            {
                throw new ArgumentNullException(nameof(original));
            }

            T clone = Object.Instantiate(original);
            return ScopeToApp(clone);
        }

        internal void NotifyQuitting()
        {
            Logging.Debug($"{this} is quitting.");
            BootstrapState = AppBootstrapState.Quitting;
            Dispose();
        }

        internal void NotifyResetting()
        {
            Logging.Debug($"{this} is resetting.");
            BootstrapState = AppBootstrapState.Resetting;
            Dispose();
            foreach (Object appScopedObject in _appScopedObjects)
            {
                if (appScopedObject.IsNotNull())
                {
                    Object.Destroy(appScopedObject);
                }
            }

            _appScopedObjects.Clear();
        }

        internal virtual Awaitable BootstrapAsync()
        {
            CreateTimestamp = DateTimeOffset.Now;
            _bootstrapState = AppBootstrapState.None;
            SessionGuid = Guid.NewGuid();
            ServiceLocator = new ServiceLocator();
            TaskScheduler = new TaskScheduler();
            return AwaitableUtility.Completed;
        }
    }
}