using Cysharp.Threading.Tasks;

namespace BeardPhantom.Fabric.Core
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