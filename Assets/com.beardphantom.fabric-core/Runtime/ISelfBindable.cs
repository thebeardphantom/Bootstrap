using Cysharp.Threading.Tasks;

namespace BeardPhantom.Fabric.Core
{
    public interface ISelfBindable<in T> : IBindable<T>
    {
        #region Methods

        UniTask SelfBindAsync();

        #endregion
    }
}