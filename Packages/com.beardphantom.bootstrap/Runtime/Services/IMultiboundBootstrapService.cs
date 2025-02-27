using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to be locatable via multiple types.
    /// </summary>
    public interface IMultiboundBootstrapService : IBootstrapService
    {
        void GetOverrideBindingTypes(List<Type> extraTypes);
    }
}