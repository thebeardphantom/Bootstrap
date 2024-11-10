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

        private static bool KeyEquals(T key, T other)
        {
            return EqualityComparer<T>.Default.Equals(key, other);
        }

        public void Cleanup()
        {
            Maps.RemoveAll(ShouldRemove);
        }

        public void AddOrReplace(T key, RuntimeBootstrapEnvironmentAsset environment)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Cleanup();

            if (TryFindIndexForKey(key, out int keyIndex))
            {
                MappedEnvironment<T> map = Maps[keyIndex];
                if (environment == null)
                {
                    Maps.RemoveAt(keyIndex);
                }
                else
                {
                    map.Environment = environment;
                }
            }
            else if (environment != null)
            {
                Maps.Add(
                    new MappedEnvironment<T>
                    {
                        Key = key,
                        Environment = environment,
                    });
            }
        }

        public IEnumerator<MappedEnvironment<T>> GetEnumerator()
        {
            return Maps.GetEnumerator();
        }

        public bool TryFindEnvironmentForKey(T key, out RuntimeBootstrapEnvironmentAsset environment)
        {
            if (TryFindIndexForKey(key, out int keyIndex))
            {
                environment = Maps[keyIndex].Environment;
                return true;
            }

            environment = default;
            return false;
        }

        private bool TryFindIndexForKey(T key, out int keyIndex)
        {
            for (var i = 0; i < Maps.Count; i++)
            {
                MappedEnvironment<T> map = Maps[i];
                if (KeyEquals(key, map.Key))
                {
                    keyIndex = i;
                    return true;
                }
            }

            keyIndex = -1;
            return false;
        }

        private bool ShouldRemove(MappedEnvironment<T> map)
        {
            return KeyEquals(map.Key, default) || map.Environment == default;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Maps).GetEnumerator();
        }
    }
}