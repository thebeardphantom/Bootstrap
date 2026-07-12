#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        internal static void InitializeEditorDelayed<T>() where T : AppInstance, new()
        {
            Logging.Info($"{nameof(InitializeEditorDelayed)} with type {typeof(T)}.");
            var updateCount = 0;
            EditorApplication.update += Update;

            void Update()
            {
                if (Application.isPlaying)
                {
                    EditorApplication.update -= Update;
                    return;
                }

                if (updateCount > 0)
                {
                    EditorApplication.update -= Update;
                    Initialize<T>();
                }
                else
                {
                    updateCount++;
                }
            }
        }

        [InitializeOnLoadMethod]
        private static void EditorEntryPoint()
        {
            InitCommon();
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (!Application.isPlaying)
            {
                InitializeEditorDelayed<EditModeAppInstance>();
            }
        }

        private static void OnCompilationStarted(object obj)
        {
            Logging.Debug($"{nameof(OnCompilationStarted)}");
            EditorApplication.isPlaying = false;
            Quit();
            Deinitialize();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Logging.Debug($"{nameof(OnPlayModeStateChanged)}({state})");
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    Deinitialize();
                    InitializeEditorDelayed<EditModeAppInstance>();
                    break;
                }
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (TryGetInstance(out EditModeAppInstance instance))
                    {
                        instance.OnExitingEditMode();
                    }

                    Quit();
                    Deinitialize();
                    break;
                }
                case PlayModeStateChange.ExitingPlayMode:
                {
                    Quit();
                    break;
                }
            }
        }
    }
}
#endif