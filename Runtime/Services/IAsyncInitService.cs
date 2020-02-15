using System.Threading.Tasks;
using UniRx.Async;

namespace ASF.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Methods

        UniTask InitAsync();

        #endregion
    }
}