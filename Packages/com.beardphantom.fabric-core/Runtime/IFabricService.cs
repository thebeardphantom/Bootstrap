using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace BeardPhantom.Fabric.Core
{
    public interface IFabricService
    {
        #region Methods

        /// <summary>
        /// Called when this service should do "non-cooperative" work. There is no guarantee that any other services
        /// are ready by this point.
        /// </summary>
        UniTask InitServiceAsync();

        /// <summary>
        /// Called when all services have been initalized. At this point all services are considered ready for
        /// "cooperative" work. By this point all services should be considered ready"
        /// </summary>
        /// <returns></returns>
        UniTask PostInitAllServicesAsync();

        /// <summary>
        /// For services that want to be locatable via multiple types.
        /// </summary>
        void GetExtraBindableTypes(List<Type> extraTypes);

        #endregion
    }
}