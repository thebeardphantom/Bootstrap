namespace BeardPhantom.Bootstrap.SourceGen
{
    public enum SingletonAccess
    {
        /// <summary>
        /// Generates a property of the Service type.
        /// </summary>
        Property = 0,

        /// <summary>
        /// Generates a method with an out parameter of the Service type.
        /// </summary>
        OutMethod = 1,
    }
}