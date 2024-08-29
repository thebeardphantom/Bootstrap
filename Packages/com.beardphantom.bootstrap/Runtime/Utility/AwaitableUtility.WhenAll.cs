using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Pool;

namespace BeardPhantom.Bootstrap
{
    public static partial class AwaitableUtility
    {
        public static async Awaitable WhenAll(IEnumerable<Awaitable> awaitables, CancellationToken cancellationToken = default)
        {
            using PooledObject<List<Awaitable>> _ = ListPool<Awaitable>.Get(out List<Awaitable> list);
            list.AddRange(awaitables);
            bool complete = AreAllTasksComplete(list);
            while (!complete && !cancellationToken.IsCancellationRequested)
            {
                complete = AreAllTasksComplete(list);
                if (!complete)
                {
                    await Awaitable.NextFrameAsync(cancellationToken);
                }
            }
        }

        public static async Awaitable<(T1 Result1, T2 Result2)> WhenAll<T1, T2>(Awaitable<T1> awaitable1, Awaitable<T2> awaitable2)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            return (result1, result2);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3)> WhenAll<T1, T2, T3>(Awaitable<T1> awaitable1, Awaitable<T2> awaitable2, Awaitable<T3> awaitable3)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            return (result1, result2, result3);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3, T4 Result4)> WhenAll<T1, T2, T3, T4>(Awaitable<T1> awaitable1, Awaitable<T2> awaitable2,
            Awaitable<T3> awaitable3, Awaitable<T4> awaitable4)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            T4 result4 = await awaitable4;
            return (result1, result2, result3, result4);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3, T4 Result4, T5 Result5)> WhenAll<T1, T2, T3, T4, T5>(Awaitable<T1> awaitable1, Awaitable<T2> awaitable2,
            Awaitable<T3> awaitable3, Awaitable<T4> awaitable4, Awaitable<T5> awaitable5)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            T4 result4 = await awaitable4;
            T5 result5 = await awaitable5;
            return (result1, result2, result3, result4, result5);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3, T4 Result4, T5 Result5, T6 Result6)> WhenAll<T1, T2, T3, T4, T5, T6>(Awaitable<T1> awaitable1,
            Awaitable<T2> awaitable2, Awaitable<T3> awaitable3, Awaitable<T4> awaitable4, Awaitable<T5> awaitable5, Awaitable<T6> awaitable6)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            T4 result4 = await awaitable4;
            T5 result5 = await awaitable5;
            T6 result6 = await awaitable6;
            return (result1, result2, result3, result4, result5, result6);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3, T4 Result4, T5 Result5, T6 Result6, T7 Result7)> WhenAll<T1, T2, T3, T4, T5, T6, T7>(
            Awaitable<T1> awaitable1, Awaitable<T2> awaitable2, Awaitable<T3> awaitable3, Awaitable<T4> awaitable4, Awaitable<T5> awaitable5, Awaitable<T6> awaitable6,
            Awaitable<T7> awaitable7)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            T4 result4 = await awaitable4;
            T5 result5 = await awaitable5;
            T6 result6 = await awaitable6;
            T7 result7 = await awaitable7;
            return (result1, result2, result3, result4, result5, result6, result7);
        }

        public static async Awaitable<(T1 Result1, T2 Result2, T3 Result3, T4 Result4, T5 Result5, T6 Result6, T7 Result7, T8 Result8)> WhenAll<T1, T2, T3, T4, T5, T6, T7, T8>(
            Awaitable<T1> awaitable1, Awaitable<T2> awaitable2, Awaitable<T3> awaitable3, Awaitable<T4> awaitable4, Awaitable<T5> awaitable5, Awaitable<T6> awaitable6,
            Awaitable<T7> awaitable7, Awaitable<T8> awaitable8)
        {
            T1 result1 = await awaitable1;
            T2 result2 = await awaitable2;
            T3 result3 = await awaitable3;
            T4 result4 = await awaitable4;
            T5 result5 = await awaitable5;
            T6 result6 = await awaitable6;
            T7 result7 = await awaitable7;
            T8 result8 = await awaitable8;
            return (result1, result2, result3, result4, result5, result6, result7, result8);
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