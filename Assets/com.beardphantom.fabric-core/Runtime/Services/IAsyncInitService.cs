using System.Threading.Tasks;
using UniRx.Async;

namespace Fabric.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Methods

        UniTask InitAsync();

        #endregion
    }
}