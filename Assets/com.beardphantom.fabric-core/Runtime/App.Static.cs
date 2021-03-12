using System.Diagnostics;

namespace BeardPhantom.Fabric.Core
{
    public partial class App
    {
        #region Fields

        private static App _instance;

        #endregion

        #region Properties

        public static App Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new App();
                }

                return _instance;
            }
        }

        #endregion

        #region Methods

        [Conditional("UNITY_EDITOR")]
        public static void CleanupEditorOnly()
        {
            _instance?.Dispose();
            _instance = null;
        }

        public static T GetService<T>() where T : class
        {
            return Instance._serviceLocator.Locate<T>();
        }

        #endregion
    }
}