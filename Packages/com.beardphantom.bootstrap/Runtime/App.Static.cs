using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public sealed partial class App
    {
        #region Fields

        private static bool _isPlaying;

        private static App _instance;

        #endregion

        #region Properties

        public static App Instance => _instance ??= new App();

        public static bool IsPlaying
        {
            get
            {
#if UNITY_EDITOR
                return _isPlaying;
#else
                return true;
#endif
            }
            internal set
            {
                _isPlaying = value;
            }
        }

        public static bool IsQuitting { get; private set; }

        public static Guid SessionGuid => _instance.IsNull() ? Guid.Empty : _instance._sessionGuid;

        #endregion

        #region Methods

        public static bool TryLocate<T>(out T service) where T : class
        {
            return Instance.ServiceLocator.TryLocateService(out service);
        }

        public static T Locate<T>() where T : class
        {
            return Instance.ServiceLocator.LocateService<T>();
        }

#if UNITY_EDITOR
        public static void CleanupEditorOnly()
        {
            Debug.Log("[Bootstrapper] App CleanupEditorOnly");
            IsQuitting = false;
            IsPlaying = false;
            _instance?.Dispose();
            _instance = null;
        }
#endif

        #endregion
    }
}