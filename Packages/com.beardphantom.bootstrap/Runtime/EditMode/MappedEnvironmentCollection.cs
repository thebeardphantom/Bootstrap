#if UNITY_EDITOR
using BeardPhantom.Bootstrap.Environment;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// A serializable mapping from keys of type <typeparamref name="T"/> to <see cref="BootstrapEnvironmentAsset"/>
    /// values.
    /// </summary>
    /// <typeparam name="T">The type of the mapping's keys.</typeparam>
    [Serializable]
    public class MappedEnvironmentCollection<T> : IReadOnlyCollection<KeyValuePair<T, BootstrapEnvironmentAsset>>
    {
        /// <summary>
        /// The number of entries in the collection.
        /// </summary>
        public int Count => Maps.Count;

        /// <summary>
        /// The underlying key/value mapping.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The underlying key/value mapping.")]
        private SerializableDictionary<T, BootstrapEnvironmentAsset> Maps { get; set; } = new();

        private static bool KeyEquals(T key, T other)
        {
            return EqualityComparer<T>.Default.Equals(key, other);
        }

        /// <summary>
        /// Removes entries with a default key or a null/destroyed environment value.
        /// </summary>
        public void Cleanup()
        {
            Maps.RemoveAll(ShouldRemove);
        }

        /// <summary>
        /// Sets the environment mapped to <paramref name="key"/>, or removes the mapping if
        /// <paramref name="environment"/> is null.
        /// </summary>
        /// <param name="key">The key to map. Must not be null.</param>
        /// <param name="environment">The environment to map to <paramref name="key"/>, or null to remove the mapping.</param>
        public void AddOrReplace(T key, BootstrapEnvironmentAsset environment)
        {
            if (key.IsNull())
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

        /// <summary>
        /// Returns an enumerator over the key/value pairs in the collection.
        /// </summary>
        public IEnumerator<KeyValuePair<T, BootstrapEnvironmentAsset>> GetEnumerator()
        {
            return Maps.GetEnumerator();
        }

        /// <summary>
        /// Attempts to find the environment mapped to <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to look up.</param>
        /// <param name="environment">The mapped environment, if found.</param>
        /// <returns>True if an environment is mapped to <paramref name="key"/>; otherwise false.</returns>
        public bool TryFindEnvironmentForKey(T key, out BootstrapEnvironmentAsset environment)
        {
            return Maps.TryGetValue(key, out environment);
        }

        private bool ShouldRemove(KeyValuePair<T, BootstrapEnvironmentAsset> pair)
        {
            return KeyEquals(pair.Key, default) || !pair.Value;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Maps).GetEnumerator();
        }
    }
}
#endif