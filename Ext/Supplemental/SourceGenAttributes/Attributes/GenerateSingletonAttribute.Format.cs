namespace BeardPhantom.Bootstrap.SourceGen
{
    public partial class GenerateSingletonAttribute
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
    }
}