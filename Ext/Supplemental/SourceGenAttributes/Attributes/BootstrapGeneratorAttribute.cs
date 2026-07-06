using System;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class BootstrapGeneratorAttribute : Attribute
    {
        internal abstract string[] Imports { get; }

        internal abstract string FilenameId { get; }

        internal abstract void Generate(StringBuilder featuresStringBuilder, string className);
    }
}