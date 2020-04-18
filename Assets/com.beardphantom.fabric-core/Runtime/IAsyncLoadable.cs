using UniRx.Async;

namespace Fabric.PersistSim.Runtime
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