using System;
using System.Collections.Generic;

namespace Fabric.Core.Runtime
{
    internal static class ListPool<T>
    {
        #region Types

        internal class PooledList : IDisposable
        {
            #region Fields

            public readonly List<T> List;

            #endregion

            #region Constructors

            public PooledList(int capacity)
            {
                List = capacity >= 0 ? new List<T>(capacity) : new List<T>();
            }

            #endregion

            #region Methods

            /// <inheritdoc />
            public void Dispose()
            {
                List.Clear();
                Return(this);
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly List<PooledList> _pool = new List<PooledList>();

        #endregion

        #region Methods

        internal static PooledList Get(out List<T> list, int capacity = -1)
        {
            if (_pool.Count == 0)
            {
                _pool.Add(new PooledList(capacity));
            }

            var lastIndex = _pool.Count - 1;
            var pooledList = _pool[lastIndex];
            _pool.RemoveAt(lastIndex);

            list = pooledList.List;
            if (capacity > list.Capacity)
            {
                list.Capacity = capacity;
            }

            return pooledList;
        }

        private static void Return(PooledList pooledList)
        {
            _pool.Add(pooledList);
        }

        #endregion
    }
}