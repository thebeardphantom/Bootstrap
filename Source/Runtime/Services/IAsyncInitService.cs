using RSG;

namespace ASF.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Properties

        IPromise InitPromise { get; }

        #endregion
    }
}