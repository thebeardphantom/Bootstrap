using System;
using System.Threading;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public abstract class AppInstance : IDisposable
    {
        public delegate void OnAppBootstrapStateChanged(in AppBootstrapState previousState, in AppBootstrapState newState);

        public event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        private AppBootstrapState _bootstrapState;

        private bool _disposed;

        private readonly CancellationTokenSource _appLifetimeCancellationTokenSource = new();

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

        public abstract bool IsQuitting { get; }

        public TaskScheduler TaskScheduler { get; private set; }

        public bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
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

        internal abstract void NotifyQuitting();

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