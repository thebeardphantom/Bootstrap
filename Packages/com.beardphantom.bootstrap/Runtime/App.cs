// #undef UNITY_EDITOR

using BeardPhantom.Bootstrap.Environment;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BeardPhantom.Bootstrap
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class App
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

                AppBootstrapState previousState = s_bootstrapState;
                s_bootstrapState = value;
                AppBootstrapStateChanged?.Invoke(previousState, value);
            }
        }

        public static ServiceLocator ServiceLocator { get; private set; }

        public static AppInitMode InitMode { get; set; }

        public static Guid SessionGuid { get; private set; }

        public static bool IsPlaying { get; private set; }

        public static bool IsQuitting { get; private set; }

        public static bool IsRunningTests { get; internal set; }

        public static AsyncTaskScheduler AsyncTaskScheduler { get; private set; }

        public static bool CanLocateServices => ServiceLocator is { CanLocateServices: true, };

        public static BootstrapEnvironmentAsset SessionEnvironment { get; private set; }

        static App()
        {
            Application.quitting += OnApplicationQuitting;
        }

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

        public static void Deinitialize(AppInitMode mode)
        {
            Logging.Info($"{nameof(DeinitializeIfInMode)}({mode})");
            try
            {
                ServiceLocator?.Dispose();
            }
            finally
            {
                AsyncTaskScheduler = null;
                SessionEnvironment = null;
                ServiceLocator = null;
                SessionGuid = Guid.Empty;
                IsPlaying = false;
                IsQuitting = false;
                BootstrapState = AppBootstrapState.None;
                InitMode = AppInitMode.Uninitialized;
                AppBootstrapStateChanged = null;
            }
        }

        public static void DeinitializeIfInMode(AppInitMode mode)
        {
            if (InitMode == mode)
            {
                Deinitialize(mode);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        internal static void Init()
        {
            InitMode = BootstrapUtility.IsInPlayMode() ? AppInitMode.PlayMode : AppInitMode.EditMode;
            s_bootstrapState = AppBootstrapState.None;
            IsQuitting = false;
            IsPlaying = InitMode == AppInitMode.PlayMode;
            SessionGuid = Guid.NewGuid();
            ServiceLocator = new ServiceLocator();
            AsyncTaskScheduler = new AsyncTaskScheduler();

            if (InitMode != AppInitMode.PlayMode)
            {
                return;
            }

            UpdateInPlayMode().Forget();

            if (!Application.isEditor)
            {
                Application.quitting += OnApplicationQuitting;
            }

            if (TryDetermineSessionEnvironment(out RuntimeBootstrapEnvironmentAsset environment))
            {
                SessionEnvironment = environment;
                Logging.Info($"Selected environment {SessionEnvironment}.");
                if (!environment.IsNoOp)
                {
                    environment.StartEnvironment();
                }
            }
            else
            {
                Logging.Info("No environment selected.");
            }
        }

        private static async Awaitable UpdateInPlayMode()
        {
            CancellationToken cancellationToken = Application.exitCancellationToken;
            while (Application.isPlaying)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await AsyncTaskScheduler.FlushQueueAsync(cancellationToken);
                await Awaitable.NextFrameAsync(cancellationToken);
            }
        }

        private static bool TryDetermineSessionEnvironment(out RuntimeBootstrapEnvironmentAsset environment)
        {
#if UNITY_EDITOR
            return TryDetermineSessionEnvironmentInEditor(out environment);
#else
            return TryDetermineSessionEnvironmentInBuild(out environment);
#endif
        }

        private static void OnApplicationQuitting()
        {
            IsQuitting = true;
        }
#if UNITY_EDITOR
        private static bool TryDetermineSessionEnvironmentInEditor(out RuntimeBootstrapEnvironmentAsset environment)
        {
            if (!BootstrapUtility.TryLoadEditModeState(out EditModeState editModeState))
            {
                environment = default;
                return false;
            }

            environment = editModeState.Environment;
            return editModeState.Environment != null;
        }
#else
        private static bool TryDetermineSessionEnvironmentInBuild(out RuntimeBootstrapEnvironmentAsset environment)
        {
            environment = RuntimeBootstrapEnvironmentAsset.Instance;
            return environment != null;
        }
#endif
    }
}