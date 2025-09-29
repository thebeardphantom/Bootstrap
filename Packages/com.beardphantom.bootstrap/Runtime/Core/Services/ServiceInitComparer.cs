using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    public class ServiceInitComparer : IComparer<IService>
    {
        public static readonly ServiceInitComparer Instance = new();

        public int Compare(IService x, IService y)
        {
            int xPriority = x is IServiceWithInitPriority xWithPriority ? xWithPriority.InitPriority : 0;
            int yPriority = y is IServiceWithInitPriority yWithPriority ? yWithPriority.InitPriority : 0;
            return xPriority.CompareTo(yPriority);
        }
    }
}