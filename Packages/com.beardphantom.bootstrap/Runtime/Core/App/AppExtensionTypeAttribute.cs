using System;
using System.Reflection;

namespace BeardPhantom.Bootstrap
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class AppExtensionTypeAttribute : Attribute
    {
        public readonly Type ExtensionType;

        public AppExtensionTypeAttribute(Type extensionType)
        {
            if (extensionType.IsNull())
            {
                throw new ArgumentNullException(nameof(extensionType));
            }

            if (!typeof(IAppExtension).IsAssignableFrom(extensionType))
            {
                throw new ArgumentException($"{extensionType} must implement {nameof(IAppExtension)}.");
            }

            ConstructorInfo parameterlessCtor = extensionType.GetConstructor(Type.EmptyTypes);

            if (parameterlessCtor.IsNull() || !parameterlessCtor.IsPublic)
            {
                throw new ArgumentException("The extension type must have a public parameterless constructor.");
            }

            ExtensionType = extensionType;
        }
    }
}