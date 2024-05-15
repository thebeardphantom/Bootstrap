namespace BeardPhantom.Bootstrap
{
    public static class BootstrapUtility
    {
        #region Methods

        public static void GetDefaultBoostrapHandlers(
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