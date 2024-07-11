namespace BeardPhantom.Bootstrap
{
    public static partial class BootstrapUtility
    {
        public static void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            preHandler = EditorBootstrapHandler.Instance;
            postHandler = EditorBootstrapHandler.Instance;
#else
            preHandler = BuildBootstrapHandler.Instance;
            postHandler = BuildBootstrapHandler.Instance;
#endif
        }
    }
}