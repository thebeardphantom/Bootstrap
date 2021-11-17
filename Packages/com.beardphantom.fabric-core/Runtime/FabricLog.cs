using UnityEngine;

namespace BeardPhantom.Fabric.Core
{
    public static class FabricLog
    {
        #region Fields

        public static ILogger Logger;

        #endregion

        #region Constructors

        static FabricLog()
        {
            RestoreDefaultLogger();
        }

        #endregion

        #region Methods

        public static void RestoreDefaultLogger()
        {
            Logger = new Logger(Debug.unityLogger.logHandler);
        }

        #endregion
    }
}