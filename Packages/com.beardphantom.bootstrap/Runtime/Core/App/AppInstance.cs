using System;
using System.Threading;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public abstract class AppInstance : IDisposable
    {
        public delegate void OnAppBootstrapStateChanged(in AppBootstrapState previousState, in AppBootstrapState newState);

        public event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        private readonly CancellationTokenSource _appLifetimeCancellationTokenSource = new();

        private AppBootstrapState _bootstrapState;

        public CancellationToken AppLifetimeCancellationToken => _appLifetimeCancellationTokenSource.Token;

        public abstract ServiceListAsset ActiveServiceListAsset { get; }

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

        public ServiceLocator ServiceLocator { get; private set; }

        public DateTimeOffset CreateTimestamp { get; private set; }

        public Guid SessionGuid { get; private set; }

        public bool IsPlaying { get; protected set; }

        public bool IsRunningTests { get; internal set; }

        public virtual bool IsQuitting => BootstrapState == AppBootstrapState.Quitting;

        public virtual bool IsResetting => BootstrapState == AppBootstrapState.Resetting;

        public TaskScheduler TaskScheduler { get; private set; }

        public bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        protected bool Disposed { get; private set; }

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

        public bool TryLocate<T>(out T service) where T : class
        {
            return ServiceLocator.TryLocateService(out service);
        }

        public T Locate<T>() where T : class
        {
            return ServiceLocator.LocateService<T>();
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