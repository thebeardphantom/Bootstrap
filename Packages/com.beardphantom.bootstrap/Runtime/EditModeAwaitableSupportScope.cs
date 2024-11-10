// #undef UNITY_EDITOR

using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.Assertions;
#endif

namespace BeardPhantom.Bootstrap
{
    public readonly partial struct EditModeAwaitableSupportScope : IDisposable
    {
        public static EditModeAwaitableSupportScope Create()
        {
            CreateInEditMode();
            return new EditModeAwaitableSupportScope();
        }

        static partial void CreateInEditMode();

        static partial void DisposeInEditMode();

        /// <inheritdoc />
        public void Dispose()
        {
            DisposeInEditMode();
        }
    }

#if UNITY_EDITOR
    public readonly partial struct EditModeAwaitableSupportScope
    {
        private static int s_counter;

        private static void OnUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

        static partial void CreateInEditMode()
        {
            if (s_counter == 0)
            {
                EditorApplication.update += OnUpdate;
            }

            s_counter++;
        }

        static partial void DisposeInEditMode()
        {
            Assert.AreNotEqual(0, s_counter, "_counter != 0");
            s_counter--;
            if (s_counter == 0)
            {
                EditorApplication.update -= OnUpdate;
            }
        }
    }
#endif
}