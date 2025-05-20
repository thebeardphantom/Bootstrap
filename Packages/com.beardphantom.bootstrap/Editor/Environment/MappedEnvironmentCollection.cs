using BeardPhantom.Bootstrap.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Bootstrap.Editor.Environment
{
    [Serializable]
    public class MappedEnvironmentCollection<T> : IReadOnlyCollection<KeyValuePair<T, BootstrapEnvironmentAsset>>
    {
        public int Count => Maps.Count;

        [field: SerializeField]
        private SerializableDictionary<T, BootstrapEnvironmentAsset> Maps { get; set; } = new();

        private static bool KeyEquals(T key, T other)
        {
            return EqualityComparer<T>.Default.Equals(key, other);
        }

        public void Cleanup()
        {
            Maps.RemoveAll(ShouldRemove);
        }

        public void AddOrReplace(T key, BootstrapEnvironmentAsset environment)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            Cleanup();
            if (environment)
            {
                Maps[key] = environment;
            }
            else
            {
                Maps.Remove(key);
            }
        }

        public IEnumerator<KeyValuePair<T, BootstrapEnvironmentAsset>> GetEnumerator()
        {
            return Maps.GetEnumerator();
        }

        public bool TryFindEnvironmentForKey(T key, out BootstrapEnvironmentAsset environment)
        {
            return Maps.TryGetValue(key, out environment);
        }

        private bool ShouldRemove(KeyValuePair<T, BootstrapEnvironmentAsset> pair)
        {
            return KeyEquals(pair.Key, default) || !pair.Value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Maps).GetEnumerator();
        }
    }
}