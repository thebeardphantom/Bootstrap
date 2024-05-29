namespace BeardPhantom.Bootstrap
{
    public static partial class BootstrapUtility
    {
        #region Methods

        public static void GetDefaultBootstrapHandlers(
            out IPreBootstrapHandler preHandler,
            out IPostBootstrapHandler postHandler)
        {
#if UNITY_EDITOR
            var editorBootstrapHandler = new EditorBootstrapHandler();
            preHandler = editorBootstrapHandler;
            postHandler = editorBootstrapHandler;
#else
            var bootstrapHandler = new BuildBootstrapHandler();
            preHandler = bootstrapHandler;
            postHandler = bootstrapHandler;
#endif
        }

        #endregion
    }
}