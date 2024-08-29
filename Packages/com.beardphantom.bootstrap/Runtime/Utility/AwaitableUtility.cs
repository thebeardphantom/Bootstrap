using System.Threading.Tasks;
using UnityEngine;

namespace BeardPhantom.Bootstrap
{
    public static partial class AwaitableUtility
    {
        public static async Awaitable<T> FromResult<T>(T result)
        {
            await new ValueTask();
            return result;
        }

        public static async Awaitable GetCompleted()
        {
            await new ValueTask();
        }

        public static async void Forget(this Awaitable awaitable)
        {
            await awaitable;
        }
    }
}