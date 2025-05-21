using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public abstract class AppInstance : IDisposable
    {
        public delegate void OnAppBootstrapStateChanged(in AppBootstrapState previousState, in AppBootstrapState newState);

        public event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        private AppBootstrapState _bootstrapState;

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

        public ServiceLocator ServiceLocator { get; set; }

        public Guid SessionGuid { get; private set; }

        public bool IsPlaying { get; protected set; }

        public bool IsRunningTests { get; internal set; }

        public abstract bool IsQuitting { get; }
        
        public TaskScheduler TaskScheduler { get; private set; }

        public bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        public virtual void Dispose()
        {
            ServiceLocator.Dispose();
        }

        public bool TryLocate<T>(out T service) where T : class
        {
            if (CanLocateServices)
            {
                return ServiceLocator.TryLocateService(out service);
            }

            service = null;
            return false;
        }

        public T Locate<T>() where T : class
        {
            Assert.IsTrue(CanLocateServices, "CanLocateServices");
            return ServiceLocator.LocateService<T>();
        }

        internal virtual Awaitable BootstrapAsync()
        {
            _bootstrapState = AppBootstrapState.None;
            SessionGuid = Guid.NewGuid();
            ServiceLocator = new ServiceLocator();
            TaskScheduler = new TaskScheduler();
            return AwaitableUtility.GetCompleted();
        }
    }
}