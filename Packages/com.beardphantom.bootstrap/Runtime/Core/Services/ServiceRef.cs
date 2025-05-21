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
                if (!App.TryGetInstance(out AppInstance instance))
                {
                    ClearValue();
                    return null;
                }

                Guid appSessionGuid = instance.SessionGuid;
                if (_value.IsNotNull() && _sessionGuid == appSessionGuid)
                {
                    return _value;
                }

                if (App.TryLocate(out _value))
                {
                    _hasValue = true;
                    _sessionGuid = appSessionGuid;
                }
                else
                {
                    ClearValue();
                }

                return _value;
            }
        }

        public bool TryGetValue(out T value)
        {
            value = Value;
            return _hasValue;
        }

        private void ClearValue()
        {
            _value = null;
            _hasValue = false;
            _sessionGuid = Guid.Empty;
        }
    }
}