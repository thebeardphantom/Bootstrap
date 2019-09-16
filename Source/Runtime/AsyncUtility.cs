using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ASF.Core.Runtime
{
    public static class AsyncUtility
    {
        #region Methods

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
                taskCompletion.SetResult(operation);
            };
            return taskCompletion.Task;
        }

        #endregion
    }
}