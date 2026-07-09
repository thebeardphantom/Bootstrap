#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
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
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            CompilationPipeline.compilationFinished += OnCompilationFinished;

            if (!Application.isPlaying)
            {
                InitializeEditorDelayed<EditModeAppInstance>();
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            Logging.Debug($"{nameof(OnPlayModeStateChanged)}({state})");
            switch (state)
            {
                case PlayModeStateChange.EnteredEditMode:
                {
                    if (TryGetInstance(out AppInstance instance))
                    {
                        instance.OnEnteringEditMode();
                    }

                    Deinitialize();
                    InitializeEditorDelayed<EditModeAppInstance>();
                    break;
                }
                case PlayModeStateChange.EnteredPlayMode:
                {
                    if (TryGetInstance(out AppInstance instance))
                    {
                        instance.OnEnteringPlaymode();
                    }

                    break;
                }
                case PlayModeStateChange.ExitingEditMode:
                {
                    if (TryGetInstance(out AppInstance instance))
                    {
                        instance.OnExitingEditMode();
                    }

                    break;
                }
                case PlayModeStateChange.ExitingPlayMode:
                {
                    if (TryGetInstance(out AppInstance instance))
                    {
                        instance.OnExitingPlaymode();
                    }

                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }

        private static void OnCompilationFinished(object obj)
        {
            Deinitialize();
        }
    }
}
#endif