namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to control their relative ordering during service initialization.
    /// </summary>
    public interface IServiceWithInitPriority : IService
    {
        /// <summary>
        /// The priority used to order this service relative to other services during initialization. Lower
        /// values are initialized first.
        /// </summary>
        int InitPriority { get; }
    }
}