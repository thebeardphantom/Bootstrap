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
                    Logging.Info($"{nameof(InitializeEditorDelayed)} with type {typeof(T)}.");
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
            if (Application.isPlaying)
            {
                return;
            }

            CompilationPipeline.compilationFinished += OnCompilationFinished;
            InitializeEditorDelayed<EditModeAppInstance>();
        }

        private static void OnCompilationFinished(object obj)
        {
            Deinitialize();
        }
    }
}
#endif
