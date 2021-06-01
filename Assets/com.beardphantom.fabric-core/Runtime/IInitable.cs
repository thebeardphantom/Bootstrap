using Cysharp.Threading.Tasks;

namespace Fabric.Core.Runtime
{
    public interface IInitable
    {
        #region Methods

        UniTask InitAsync();

        #endregion
    }
}