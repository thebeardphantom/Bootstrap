using System;
using UnityEditor;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.Editor
{
    public readonly struct EditModePlayerLoopScope : IDisposable
    {
        private static int s_counter;

        public static EditModePlayerLoopScope Create()
        {
            if (s_counter == 0)
            {
                EditorApplication.update += OnUpdate;
            }

            s_counter++;
            return new EditModePlayerLoopScope();
        }

        private static void OnUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Assert.AreNotEqual(0, s_counter, "_counter != 0");
            s_counter--;
            if (s_counter == 0)
            {
                EditorApplication.update -= OnUpdate;
            }
        }
    }
}