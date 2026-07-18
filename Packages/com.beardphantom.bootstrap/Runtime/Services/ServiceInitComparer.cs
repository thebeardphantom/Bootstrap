using System.Collections.Generic;

namespace BeardPhantom.Bootstrap
{
    /// <summary>
    /// Compares <see cref="IService"/> instances by their <see cref="IServiceWithInitPriority.InitPriority"/>,
    /// treating services that don't implement <see cref="IServiceWithInitPriority"/> as priority 0.
    /// </summary>
    public class ServiceInitComparer : IComparer<IService>
    {
        /// <summary>
        /// Shared singleton instance of the comparer.
        /// </summary>
        public static readonly ServiceInitComparer Instance = new();

        public int Compare(IService x, IService y)
        {
            int xPriority = x is IServiceWithInitPriority xWithPriority ? xWithPriority.InitPriority : 0;
            int yPriority = y is IServiceWithInitPriority yWithPriority ? yWithPriority.InitPriority : 0;
            return xPriority.CompareTo(yPriority);
        }
    }
}