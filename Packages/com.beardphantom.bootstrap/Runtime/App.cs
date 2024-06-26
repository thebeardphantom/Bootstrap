﻿using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        #region Types

        public delegate void OnAppBootstrapStateChanged(AppBootstrapState previousState, AppBootstrapState newState);

        #endregion

        #region Events

        public static event OnAppBootstrapStateChanged AppBootstrapStateChanged;

        #endregion

        #region Fields

        private static AppBootstrapState _bootstrapState;

        #endregion

        #region Properties

        public static AppBootstrapState BootstrapState
        {
            get => _bootstrapState;
            internal set
            {
                if (_bootstrapState == value)
                {
                    return;
                }

                var previousState = _bootstrapState;
                _bootstrapState = value;
                AppBootstrapStateChanged?.Invoke(previousState, value);
            }
        }

        public static ServiceLocator ServiceLocator { get; internal set; }

        public static Guid SessionGuid { get; private set; }

        public static bool IsPlaying { get; private set; }

        public static bool IsQuitting { get; private set; }

        public static bool CanLocateServices => BootstrapState >= AppBootstrapState.ServiceLateInit;

        #endregion

        #region Methods

        public static bool TryLocate<T>(out T service) where T : class
        {
            return ServiceLocator.TryLocateService(out service);
        }

        public static T Locate<T>() where T : class
        {
            return ServiceLocator.LocateService<T>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _bootstrapState = AppBootstrapState.None;
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

        #endregion
    }
}