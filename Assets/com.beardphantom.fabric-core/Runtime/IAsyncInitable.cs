using Cysharp.Threading.Tasks;

namespace Fabric.Core.Runtime
{
    public interface IAsyncInitable<in T>
    {
        #region Methods

        UniTask InitAsync(T initData);

        #endregion
    }
}