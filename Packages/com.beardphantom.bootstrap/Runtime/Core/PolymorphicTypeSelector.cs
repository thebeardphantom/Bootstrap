using System;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public class PolymorphicTypeSelectorAttribute : PropertyAttribute
    {
        public readonly Type BaseType;

        public PolymorphicTypeSelectorAttribute(Type baseType)
        {
            BaseType = baseType;
        }
    }
}