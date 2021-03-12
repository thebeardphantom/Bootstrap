using Cysharp.Threading.Tasks;

namespace BeardPhantom.Fabric.Core
{
    public interface IBindable<in T>
    {
        #region Methods

        UniTask BindAsync(T initData);

        UniTask UnbindAsync();

        #endregion
    }
}