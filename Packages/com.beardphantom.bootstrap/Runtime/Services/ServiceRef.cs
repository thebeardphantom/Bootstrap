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
                    if (App.TryLocate(out _value))
                    {
                        _sessionGuid = appSessionGuid;
                    }
                }

                return _value;
            }
        }
    }
}