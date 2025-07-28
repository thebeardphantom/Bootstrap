using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BeardPhantom.Bootstrap.SourceGen
{
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public class GenerateSingletonAttribute : BootstrapGeneratorAttribute
    {
        private const string Format = @"
        private static {0} _serviceInstance;
        private static bool _serviceInstanceFound;
                            
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
        }}

#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod]
        private static void ClearInstance()
        {{
            _serviceInstance = null;
            _serviceInstanceFound = false;

            void OnQuit()
            {{
                Application.quitting -= OnQuit;
                _serviceInstance = null;
                _serviceInstanceFound = false;
            }}

            Application.quitting += OnQuit;
        }}
#endif
";

        internal override void Generate(StringBuilder importsStringBuilder, StringBuilder featuresStringBuilder, string className)
        {
            importsStringBuilder.AppendLine("using UnityEngine;");
            importsStringBuilder.AppendLine("using BeardPhantom.Bootstrap;");
            featuresStringBuilder.AppendFormat(Format, className);
        }
    }
}