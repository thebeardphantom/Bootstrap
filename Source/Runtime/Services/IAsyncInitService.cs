using System.Threading.Tasks;

namespace ASF.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Methods

        Task InitAsync();

        #endregion
    }
}