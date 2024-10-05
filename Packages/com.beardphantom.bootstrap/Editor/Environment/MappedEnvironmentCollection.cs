using BeardPhantom.Bootstrap.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [Serializable]
    public class MappedEnvironmentCollection<T> : IReadOnlyList<MappedEnvironment<T>>
    {
        public int Count => Maps.Count;

        [field: SerializeField]
        private List<MappedEnvironment<T>> Maps { get; set; } = new();

        public MappedEnvironment<T> this[int index] => Maps[index];

        public void Cleanup()
        {
            Maps.RemoveAll(ShouldRemove);
        }

        public void AddOrReplace(T key, RuntimeBootstrapEnvironmentAsset environment)
        {
            Cleanup();
            foreach (MappedEnvironment<T> map in Maps)
            {
                if (EqualityComparer<T>.Default.Equals(map.Key, key))
                {
                    map.Environment = environment;
                    return;
                }
            }

            Maps.Add(
                new MappedEnvironment<T>
                {
                    Key = key,
                    Environment = environment,
                });
        }

        public IEnumerator<MappedEnvironment<T>> GetEnumerator()
        {
            return Maps.GetEnumerator();
        }

        private bool ShouldRemove(MappedEnvironment<T> map)
        {
            return EqualityComparer<T>.Default.Equals(map.Key, default) || map.Environment == default;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Maps).GetEnumerator();
        }
    }
}