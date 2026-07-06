using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class GenerateSingletonAttribute : BootstrapGeneratorAttribute
    {
        private const string PropertyFormatStr = @"
        public static {0} Instance
        {{
            get
            {{
                if (_serviceInstanceFound)
                {{
                    Debug.Assert(_serviceInstance != null);
                }}
                else
                {{
                    _serviceInstance = App.Locate<{0}>();
                    _serviceInstanceFound = true;
                }}

                return _serviceInstance;
            }}
        }}";

        private const string OutMethodFormatStr = @"
        public static void GetInstance(out {0} instance)
        {{
            if (_serviceInstanceFound)
            {{
                Debug.Assert(_serviceInstance != null);
            }}
            else
            {{
                _serviceInstance = App.Locate<{0}>();
                _serviceInstanceFound = true;
            }}

            instance = _serviceInstance;
        }}";

        private const string Format = @"
        private static {0} _serviceInstance;
        private static bool _serviceInstanceFound;

        {1}

#if UNITY_EDITOR
#if UNITY_6000_5_OR_NEWER
        [OnExitingPlayMode]
        private static void OnExitingPlayMode()
        {{
            _serviceInstance = null;
            _serviceInstanceFound = false;
        }}
#else
        [RuntimeInitializeOnLoadMethod]
        private static void ClearInstance()
        {{
            _serviceInstance = null;
            _serviceInstanceFound = false;
            
            UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
            {{
                if (state == UnityEditor.PlayModeStateChange.EnteredEditMode)
                {{
                    UnityEditor.EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                    _serviceInstance = null;
                    _serviceInstanceFound = false;
                }}
            }}
        }}
#endif
#endif
";

        public readonly SingletonAccess Access;

        internal override string[] Imports { get; } =
        {
            "UnityEngine",
            "BeardPhantom.Bootstrap",
        };

        internal override string FilenameId => "Singleton";

        public GenerateSingletonAttribute() : this(SingletonAccess.Property) { }

        public GenerateSingletonAttribute(SingletonAccess access)
        {
            Access = access;
        }

        internal override void Generate(StringBuilder featuresStringBuilder, string className)
        {
            string accessFormatStr = Access == SingletonAccess.Property ? PropertyFormatStr : OutMethodFormatStr;
            string accessStr = string.Format(accessFormatStr, className);
            featuresStringBuilder.AppendFormat(Format, className, accessStr);
        }
    }
}