using UniRx.Async;

namespace Fabric.Core.Runtime
{
    public interface IAsyncLoadable
    {
        #region Methods

        UniTask LoadAsync();

        #endregion
    }

    public interface IAsyncLoadable<T>
    {
        #region Methods

        UniTask<T> LoadAsync();

        #endregion
    }
}