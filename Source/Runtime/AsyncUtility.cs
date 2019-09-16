using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ASF.Core.Runtime
{
    public static class AsyncUtility
    {
        #region Methods

        public static async Task WaitForFrames(int frames = 1)
        {
            Assert.IsTrue(frames > 0, "Frames must be > 0");
            await PlayerLoopHook.GetWaitForFramesTask(frames);
        }

        public static Task<AsyncOperation> ToTask(this AsyncOperation asyncOp)
        {
            var taskCompletion = new TaskCompletionSource<AsyncOperation>();
            asyncOp.completed += operation =>
            {
                taskCompletion.SetResult(operation);
            };
            return taskCompletion.Task;
        }

        public static Task<AsyncOperationHandle<T>> ToTask<T>(this AsyncOperationHandle<T> asyncOp)
        {
            var taskCompletion = new TaskCompletionSource<AsyncOperationHandle<T>>();
            asyncOp.Completed += operation =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    taskCompletion.SetResult(operation);
                }
                else
                {
                    taskCompletion.SetException(operation.OperationException);
                }
            };
            return taskCompletion.Task;
        }

        #endregion
    }
}