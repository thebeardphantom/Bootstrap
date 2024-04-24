using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap.Services
{
    /// <summary>
    /// For services that want to be locatable via multiple types.
    /// </summary>
    public interface IMultiboundBootstrapService : IBootstrapService
    {
        #region Methods

        void GetExtraBindableTypes(List<Type> extraTypes);

        #endregion
    }
}