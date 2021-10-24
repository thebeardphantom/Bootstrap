using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Fabric.Core.Runtime
{
    public interface IFabricService
    {
        #region Methods

        UniTask OnCreateServiceAsync();

        UniTask OnAllServicesCreatedAsync();

        void GetExtraBindableTypes(List<Type> extraTypes);

        #endregion
    }
}