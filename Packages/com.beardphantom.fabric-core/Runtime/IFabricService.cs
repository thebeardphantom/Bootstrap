using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BeardPhantom.Fabric.Core
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