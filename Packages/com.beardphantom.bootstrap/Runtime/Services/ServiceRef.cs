using System;

namespace BeardPhantom.Bootstrap
{
    public class ServiceRef<T> where T : class
    {
        private T _value;

        private Guid _sessionGuid;

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
    }
}