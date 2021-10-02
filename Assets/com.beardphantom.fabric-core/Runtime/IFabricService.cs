using Cysharp.Threading.Tasks;

namespace Fabric.Core.Runtime
{
    public interface IFabricService
    {
        #region Methods

        UniTask OnCreateServiceAsync();

        UniTask OnAllServicesCreatedAsync();

        #endregion
    }
}