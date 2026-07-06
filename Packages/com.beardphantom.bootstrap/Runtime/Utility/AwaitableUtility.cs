using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    internal static class AwaitableUtility
    {
        public static Awaitable Completed => GetCompleted();

        public static void Forget(this Awaitable awaitable, bool silenceOperationCancelledExceptions = true)
        {
            _ = ForgetImpl(awaitable, silenceOperationCancelledExceptions);
        }

        public static void Forget<T>(this Awaitable<T> awaitable, bool silenceOperationCancelledExceptions = true)
        {
            _ = ForgetImpl(awaitable, silenceOperationCancelledExceptions);
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

        /// <summary>
        /// Gets an Awaitable that is already completed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "AsyncMethodWithoutAwait")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async Awaitable GetCompleted() { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        private static async Awaitable ForgetImpl(Awaitable awaitable, bool silenceOperationCancelledExceptions)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException) when (silenceOperationCancelledExceptions) { }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static async Awaitable ForgetImpl<T>(Awaitable<T> awaitable, bool silenceOperationCancelledExceptions)
        {
            try
            {
                await awaitable;
            }
            catch (OperationCanceledException) when (silenceOperationCancelledExceptions) { }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    internal static class AwaitableUtility<T>
    {
        public static Awaitable<T> Completed => FromResult();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("ReSharper", "AsyncMethodWithoutAwait")]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Awaitable<T> FromResult(T result = default)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return result;
        }
    }
}