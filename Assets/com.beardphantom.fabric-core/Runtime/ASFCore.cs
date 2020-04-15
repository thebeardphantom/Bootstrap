using UnityEngine;

namespace ASF.Core.Runtime
{
    public static class ASFCore
    {
        #region Fields

        public static ILogger Logger;

        #endregion

        #region Constructors

        static ASFCore()
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