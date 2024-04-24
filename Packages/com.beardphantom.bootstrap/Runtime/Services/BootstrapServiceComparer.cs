using System.Collections.Generic;

namespace BeardPhantom.Bootstrap.Services
{
    public class BootstrapServiceComparer : IComparer<IBootstrapService>
    {
        #region Fields

        public static readonly BootstrapServiceComparer Instance = new();

        #endregion

        #region Methods

        public int Compare(IBootstrapService x, IBootstrapService y)
        {
            var priorityX = 0;
            var priorityY = 0;
            if (x is IBootstrapServiceWithPriority serviceWithPriorityX)
            {
                priorityX = serviceWithPriorityX.Priority;
            }

            if (y is IBootstrapServiceWithPriority serviceWithPriorityY)
            {
                priorityY = serviceWithPriorityY.Priority;
            }

            return priorityX.CompareTo(priorityY);
        }

        #endregion
    }
}