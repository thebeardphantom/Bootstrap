namespace BeardPhantom.Bootstrap.SourceGen
{
    public partial class GenerateSingletonAttribute
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
";
    }
}