using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to be locatable via multiple types.
    /// </summary>
    public interface IServiceWithCustomBindings : IService
    {
        void GetCustomBindings(List<Type> bindingTypes, out bool autoIncludeDeclaredType);
    }
}
