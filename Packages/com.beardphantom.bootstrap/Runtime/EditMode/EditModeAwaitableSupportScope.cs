#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.EditMode
{
    public readonly struct EditModeAwaitableSupportScope : IDisposable
    {
        private static int s_counter;

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