using System;

namespace BeardPhantom.Bootstrap
{
    public class ServiceNotFoundException : Exception
    {
        public ServiceNotFoundException(Type serviceType) : base($"Service of type {serviceType} not found.") { }
    }
}