﻿#if UNITY_EDITOR
using BeardPhantom.Bootstrap.EditMode;
using UnityEditor;
using UnityEditor.Compilation;

namespace BeardPhantom.Bootstrap
{
    public static partial class App
    {
        internal static void InitializeEditorDelayed<T>() where T : AppInstance, new()
        {
            EditorApplication.delayCall += () =>
            {
                Logging.Info($"{nameof(InitializeEditorDelayed)} with type {typeof(T)}.");
                Initialize<T>();
            };
        }

        [InitializeOnLoadMethod]
        private static void EditorEntryPoint()
        {
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