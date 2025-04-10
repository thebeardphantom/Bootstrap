using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class AwaitableUtility
    {
        public static async Awaitable GetCompleted()
        {
            await new ValueTask();
        }

        public static async void Forget(this Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException) { }
        }

        public static async Awaitable<T> FromResult<T>(T result)
        {
            await GetCompleted();
            return result;
        }

        public static async Awaitable WaitUntil(Func<bool> predicate, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            while (!predicate())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }
    }
}