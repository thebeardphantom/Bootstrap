using System;
using System.Collections.Generic;

namespace Fabric.Core.Runtime
{
    public interface IBindableService
    {
        #region Properties

        void GetExtraBindableTypes(List<Type> extraTypes);

        #endregion
    }
}