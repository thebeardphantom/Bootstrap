using System;
using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// For services that want to be locatable via multiple types.
    /// </summary>
    public interface IServiceWithCustomBindings : IService
    {
        /// <summary>
        /// Populates the set of additional types this service should be locatable via.
        /// </summary>
        /// <param name="bindingTypes">The list to add additional binding types to.</param>
        /// <param name="autoIncludeDeclaredType">Whether the service's own declared type should also be
        /// registered as a binding, in addition to the types added to <paramref name="bindingTypes"/>.</param>
        void GetCustomBindings(List<Type> bindingTypes, out bool autoIncludeDeclaredType);
    }
}