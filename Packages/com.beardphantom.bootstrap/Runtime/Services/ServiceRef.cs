namespace BeardPhantom.Bootstrap
{
    public static class ServiceRef<T> where T : class
    {
        private static T s_instance;

        private static bool s_subscribed;

        public static T Instance
        {
            get
            {
                if (s_instance.IsNotNull())
                {
                    return s_instance;
                }

                EnsureSubscribed();
                s_instance = App.Locate<T>();
                return s_instance;
            }
        }

        public static bool TryGetInstance(out T instance)
        {
            if (s_instance.IsNull())
            {
                EnsureSubscribed();
                App.TryLocate(out s_instance);
            }

            instance = s_instance;
            return s_instance.IsNotNull();
        }

        private static void EnsureSubscribed()
        {
            if (s_subscribed)
            {
                return;
            }

            s_subscribed = true;
            App.Deinitialized += Clear;
        }

        private static void Clear()
        {
            s_instance = null;
        }
    }
}