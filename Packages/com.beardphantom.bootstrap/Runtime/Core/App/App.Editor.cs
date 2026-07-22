#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        /// <remarks>
        /// <c>EditorApplication.update</c> is used instead of <c>EditorApplication.delayCall</c> to work better with the
        /// Multiplayer Play Mode package, as <c>EditorApplication.delayCall</c> doesn't get called on unfocused editor
        /// applications.
        /// </remarks>
        [Conditional("UNITY_EDITOR")]
        internal static void CreateAppInstanceDelayed<T>() where T : AppInstance, new()
        {
            Logging.Info($"{nameof(CreateAppInstanceDelayed)} with type {typeof(T)}.");
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
                    CreateAppInstance<T>();
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
            Init();
            CompilationPipeline.compilationStarted += OnCompilationStarted;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (!Application.isPlaying)
            {
                CreateAppInstanceDelayed<EditModeAppInstance>();
            }
        }

        private static void OnCompilationStarted(object obj)
        {
            Logging.Debug($"{nameof(OnCompilationStarted)}");
            EditorApplication.isPlaying = false;
            Quit();
            DestroyAppInstance();
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Logging.Debug($"{nameof(OnPlayModeStateChanged)}({state})");
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    DestroyAppInstance();
                    CreateAppInstanceDelayed<EditModeAppInstance>();
                    break;
                }
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (TryGetInstance(out EditModeAppInstance instance))
                    {
                        instance.OnExitingEditMode();
                    }

                    Quit();
                    DestroyAppInstance();
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