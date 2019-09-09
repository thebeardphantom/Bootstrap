using RSG;

namespace ASFUnity.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Properties

        IPromise InitPromise { get; }

        #endregion
    }
}