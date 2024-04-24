namespace BeardPhantom.Bootstrap.Services
{
    public interface IBootstrapServiceWithPriority : IBootstrapService
    {
        #region Properties

        int Priority { get; }

        #endregion
    }
}