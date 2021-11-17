using System;
using System.Diagnostics;

namespace BeardPhantom.Fabric.Core
{
    public sealed partial class App
    {
        #region Fields

        private static App _instance;

        #endregion

        #region Properties

        public static App Instance => _instance ??= new App();

        public static Guid SessionGuid => _instance.IsNull() ? Guid.Empty : _instance._sessionGuid;

        #endregion

        #region Methods

        [Conditional("UNITY_EDITOR")]
        public static void CleanupEditorOnly()
        {
            FabricLog.Logger.Log("App CleanupEditorOnly");
            _instance?.Dispose();
            _instance = null;
        }

        public static bool TryLocate<T>(out T service) where T : class
        {
            return Instance.ServiceLocator.TryLocateService(out service);
        }

        public static T Locate<T>() where T : class
        {
            return Instance.ServiceLocator.LocateService<T>();
        }

        #endregion
    }
}