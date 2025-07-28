using System;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class BootstrapGeneratorAttribute : Attribute
    {
        internal abstract void Generate(StringBuilder importsStringBuilder, StringBuilder featuresStringBuilder, string className);
    }
}