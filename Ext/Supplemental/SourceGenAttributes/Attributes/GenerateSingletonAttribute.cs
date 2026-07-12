using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class GenerateSingletonAttribute : BootstrapGeneratorAttribute
    {
        private const string PropertyFormatStr = @"
        private static {0} Instance
        {{
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ServiceRef<{0}>.Instance;
        }}";

        private const string OutMethodFormatStr = @"
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetInstance(out {0} instance)
        {{
            instance = ServiceRef<{0}>.Instance;
        }}";

        public readonly SingletonAccessors Accessors;

        internal override string[] Imports { get; } =
        {
            "UnityEngine",
            "BeardPhantom.Bootstrap",
            "System.Runtime.CompilerServices",
        };

        internal override string FilenameId => "Singleton";

        public GenerateSingletonAttribute() : this(SingletonAccessors.Property) { }

        public GenerateSingletonAttribute(SingletonAccessors accessors)
        {
            Accessors = accessors;
            if (accessors == 0)
            {
                throw new ArgumentOutOfRangeException($"Argument {nameof(accessors)} must have at least one value set.");
            }
        }

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            if (Accessors.HasFlagFast(SingletonAccessors.Property))
            {
                featuresStringBuilder.AppendFormat(PropertyFormatStr, className);
            }

            if (Accessors.HasFlagFast(SingletonAccessors.OutMethod))
            {
                featuresStringBuilder.AppendFormat(OutMethodFormatStr, className);
            }
        }
    }
}