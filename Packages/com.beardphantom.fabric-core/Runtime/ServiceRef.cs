using System;

namespace BeardPhantom.Fabric.Core
{
    public class ServiceRef<T> where T : class
    {
        #region Fields

        private T _value;

        private Guid _sessionGuid;

        #endregion

        #region Properties

        public T Value
        {
            get
            {
                var appSessionGuid = App.SessionGuid;
                if (_value.IsNull() || _sessionGuid != appSessionGuid)
                {
                    _sessionGuid = appSessionGuid;
                    _value = App.Locate<T>();
                }

                return _value;
            }
        }

        #endregion
    }
}