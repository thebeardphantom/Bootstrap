using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fabric.Core.Runtime
{
    public static class AsyncUtility
    {
        #region Methods

        public static UniTask<AsyncOperation> ToTask(this AsyncOperation asyncOp)
        {
            var taskCompletion = new UniTaskCompletionSource<AsyncOperation>();
            asyncOp.completed += operation =>
            {
                taskCompletion.TrySetResult(operation);
            };
            return taskCompletion.Task;
        }

        public static UniTask<AsyncOperationHandle<T>> ToTask<T>(this AsyncOperationHandle<T> asyncOp)
        {
            var taskCompletion = new UniTaskCompletionSource<AsyncOperationHandle<T>>();
            asyncOp.Completed += operation =>
            {
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    taskCompletion.TrySetResult(operation);
                }
                else
                {
                    taskCompletion.TrySetException(operation.OperationException);
                }
            };
            return taskCompletion.Task;
        }

        #endregion
    }
}