using UniRx.Async;

namespace Fabric.Core.Runtime
{
    public interface ISelfAsyncInitable
    {
        #region Methods

        UniTask SelfInitAsync();

        #endregion
    }
}