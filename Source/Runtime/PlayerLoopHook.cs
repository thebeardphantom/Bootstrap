using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;

namespace ASF.Core.Runtime
{
    public static class PlayerLoopHook
    {
        #region Types

        private class FuncEntry : IComparable<FuncEntry>
        {
            #region Fields

            public int Order;
            public PlayerLoopSystem.UpdateFunction Function;

            #endregion

            #region Methods

            /// <inheritdoc />
            public int CompareTo(FuncEntry other)
            {
                return Order.CompareTo(other.Order);
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly List<FuncEntry> _callbacks = new List<FuncEntry>();

        private static readonly List<TaskCompletionSource<object>> _frameWaitTasks =
            new List<TaskCompletionSource<object>>();

        private static bool _created;

        #endregion

        #region Methods

        public static void Create()
        {
            if (_created)
            {
                return;
            }

            _created = true;
            var loop = PlayerLoop.GetDefaultPlayerLoop();
            PlayerLoopUtility.InsertUpdateLoopSystem<Update.ScriptRunDelayedTasks>(
                ref loop,
                new PlayerLoopSystem
                {
                    updateDelegate = UpdateDelayedTasks
                });
            PlayerLoopUtility.InsertUpdateLoopSystem<Update.ScriptRunBehaviourUpdate>(
                ref loop,
                new PlayerLoopSystem
                {
                    updateDelegate = UpdateAll
                });
            PlayerLoop.SetPlayerLoop(loop);
        }

        public static void Teardown()
        {
            if (_created)
            {
                _created = false;
                PlayerLoop.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
            }
        }

        public static void Register(PlayerLoopSystem.UpdateFunction func, int order = 0)
        {
            if (!_created)
            {
                Create();
            }

            _callbacks.Add(
                new FuncEntry
                {
                    Function = func,
                    Order = order
                });
            _callbacks.Sort();
        }

        public static void Unregister(PlayerLoopSystem.UpdateFunction func)
        {
            if (_created)
            {
                _callbacks.RemoveAll(c => c.Function == func);
            }
        }

        public static async Task GetWaitForFramesTask(int frames = 1)
        {
            if (!_created)
            {
                Create();
            }

            Assert.IsTrue(frames > 0, "Frames must be > 0");
            var endFrame = Time.frameCount + frames;
            var taskCompletion = new TaskCompletionSource<object>(endFrame);
            _frameWaitTasks.Add(taskCompletion);
            await taskCompletion.Task;
        }

        private static void UpdateDelayedTasks()
        {
            for (var i = _frameWaitTasks.Count - 1; i >= 0; i--)
            {
                var task = _frameWaitTasks[i];
                var endFrame = (int) task.Task.AsyncState;
                if (Time.frameCount >= endFrame)
                {
                    task.SetResult(null);
                    _frameWaitTasks.RemoveAt(i);
                }
            }
        }

        private static void UpdateAll()
        {
            foreach (var callback in _callbacks)
            {
                callback.Function();
            }
        }

        #endregion
    }
}