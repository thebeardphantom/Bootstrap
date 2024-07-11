using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        public delegate void OnAppBootstrapStateChanged(AppBootstrapState previousState, AppBootstrapState newState);

        public static event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        private static AppBootstrapState s_bootstrapState;

        public static AppBootstrapState BootstrapState
        {
            get => s_bootstrapState;
            internal set
            {
                if (s_bootstrapState == value)
                {
                    return;
                }

                var previousState = s_bootstrapState;
                s_bootstrapState = value;
                AppBootstrapStateChanged?.Invoke(previousState, value);
            }
        }

        public static ServiceLocator ServiceLocator { get; internal set; }

        public static Guid SessionGuid { get; private set; }

        public static bool IsPlaying { get; private set; }

        public static bool IsQuitting { get; private set; }

        public static bool IsRunningTests { get; set; }

        public static bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        public static bool TryLocate<T>(out T service) where T : class
        {
            if (!CanLocateServices)
            {
                service = default;
                return false;
            }

            return ServiceLocator.TryLocateService(out service);
        }

        public static T Locate<T>() where T : class
        {
            Assert.IsTrue(CanLocateServices, "CanLocateServices");
            return ServiceLocator.LocateService<T>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            s_bootstrapState = AppBootstrapState.None;
            ServiceLocator = new ServiceLocator();
            IsQuitting = false;
            IsPlaying = true;
            Application.quitting -= OnApplicationQuitting;
            Application.quitting += OnApplicationQuitting;
        }

        private static void OnApplicationQuitting()
        {
            Application.quitting -= OnApplicationQuitting;
            IsQuitting = true;
        }
    }
}