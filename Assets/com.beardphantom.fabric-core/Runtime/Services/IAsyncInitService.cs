using Cysharp.Threading.Tasks;

namespace Fabric.Core.Runtime
{
    public interface IAsyncInitService
    {
        #region Methods

        UniTask InitAsync();

        #endregion
    }
}