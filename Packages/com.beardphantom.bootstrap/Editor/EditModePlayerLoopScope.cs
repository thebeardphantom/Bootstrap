using System;
using UnityEditor;
using UnityEngine.Assertions;

namespace BeardPhantom.Bootstrap.Editor
{
    public readonly struct EditModePlayerLoopScope : IDisposable
    {
        #region Fields

        private static int _counter;

        #endregion

        #region Methods

        public static EditModePlayerLoopScope Create()
        {
            if (_counter == 0)
            {
                EditorApplication.update += OnUpdate;
            }

            _counter++;
            return new EditModePlayerLoopScope();
        }

        private static void OnUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Assert.AreNotEqual(0, _counter, "_counter != 0");
            _counter--;
            if (_counter == 0)
            {
                EditorApplication.update -= OnUpdate;
            }
        }

        #endregion
    }
}