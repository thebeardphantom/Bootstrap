using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    internal static class AwaitableUtility
    {
        public static async Awaitable WhenAll(IEnumerable<Awaitable> awaitables, CancellationToken cancellationToken = default)
        {
            using PooledObject<List<Awaitable>> _ = ListPool<Awaitable>.Get(out List<Awaitable> list);
            list.AddRange(awaitables);
            bool complete = AreAllTasksComplete(list);
            while (!complete)
            {
                cancellationToken.ThrowIfCancellationRequested();
                complete = AreAllTasksComplete(list);
                if (!complete)
                {
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
            }

            foreach (Awaitable awaitable in list)
            {
                await awaitable;
            }
        }

        public static async Awaitable<T> FromResult<T>(T result)
        {
            await GetCompleted();
            return result;
        }

        public static async Awaitable GetCompleted()
        {
            await Task.CompletedTask;
        }

        public static async void Forget(this Awaitable awaitable)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException) { }
        }

        private static bool AreAllTasksComplete(List<Awaitable> awaitables)
        {
            foreach (Awaitable awaitable in awaitables)
            {
                if (!awaitable.IsCompleted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}