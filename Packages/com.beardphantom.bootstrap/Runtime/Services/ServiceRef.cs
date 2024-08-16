using System;

namespace BeardPhantom.Bootstrap
{
    public class ServiceRef<T> where T : class
    {
        private T _value;

        private Guid _sessionGuid;

        private bool _hasValue;

        public T Value
        {
            get
            {
                var appSessionGuid = App.SessionGuid;
                if (_value.IsNull() || _sessionGuid != appSessionGuid)
                {
                    _hasValue = false;
                    if (App.TryLocate(out _value))
                    {
                        _hasValue = true;
                        _sessionGuid = appSessionGuid;
                    }
                }

                return _value;
            }
        }

        public bool TryGetValue(out T value)
        {
            value = Value;
            return _hasValue;
        }
    }
}