#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// Disposable scope that keeps the editor's player loop pumping while it is active, allowing
    /// <see cref="Awaitable"/>-based async code to make progress in edit mode. Nested scopes are reference-counted.
    /// </summary>
    public readonly struct EditModeAwaitableSupportScope : IDisposable
    {
        private static int s_counter;

        /// <summary>
        /// Creates and activates a new scope, enabling player loop updates in edit mode for as long as any scope
        /// instance remains undisposed.
        /// </summary>
        public static EditModeAwaitableSupportScope Create()
        {
            CreateInEditMode();
            return new EditModeAwaitableSupportScope();
        }

        private static void OnUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private static void CreateInEditMode()
        {
            if (s_counter == 0)
            {
                EditorApplication.update += OnUpdate;
            }

            s_counter++;
        }

        private static void DisposeInEditMode()
        {
            Assert.AreNotEqual(0, s_counter, "_counter != 0");
            s_counter--;
            if (s_counter == 0)
            {
                EditorApplication.update -= OnUpdate;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeInEditMode();
        }
    }
}
#endif