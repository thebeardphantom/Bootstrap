namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Caches a lazily-located service of type <typeparamref name="T"/>, clearing the cache when the app
    /// deinitializes.
    /// </summary>
    /// <typeparam name="T">The type of service to cache.</typeparam>
    public static class ServiceRef<T> where T : class
    {
        private static T s_instance;

        private static bool s_subscribed;

        /// <summary>
        /// The cached service instance, locating it on first access.
        /// </summary>
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

        /// <summary>
        /// Attempts to get the cached service instance, locating it if not already cached.
        /// </summary>
        /// <param name="instance">The located service, or null if not found.</param>
        /// <returns>True if the service was located; otherwise false.</returns>
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