namespace Fabric.Core.Runtime
{
    public interface IInitable<in T>
    {
        #region Methods

        void Init(T initData);

        #endregion
    }
}