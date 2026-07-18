using System;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Thrown when a service of a requested type cannot be located.
    /// </summary>
    public class ServiceNotFoundException : Exception
    {
        /// <summary>
        /// Creates a new exception for a service that could not be located.
        /// </summary>
        /// <param name="serviceType">The type of service that could not be found.</param>
        public ServiceNotFoundException(Type serviceType) : base($"Service of type {serviceType} not found.") { }
    }
}