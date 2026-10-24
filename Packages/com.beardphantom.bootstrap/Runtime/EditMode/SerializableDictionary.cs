#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeardPhantom.Bootstrap.EditMode
{
    /// <summary>
    /// A <see cref="Dictionary{TKey,TValue}" /> that Unity can serialize, by mirroring its contents into a
    /// serialized list of key/value pairs before and after serialization.
    /// </summary>
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// The dictionary's contents mirrored into a serializable list of key/value pairs.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("The dictionary's contents mirrored into a serializable list of key/value pairs.")]
        private List<SerializedPair> SerializedPairs { get; set; } = new();

        /// <summary>
        /// Removes all key/value pairs that match <paramref name="predicate" />.
        /// </summary>
        public void RemoveAll(Predicate<KeyValuePair<TKey, TValue>> predicate)
        {
            CopyToSerializedList();
            SerializedPairs.RemoveAll(WrappedPredicate);
            CopyFromSerializedList();
            return;

            bool WrappedPredicate(SerializedPair pair)
            {
                return predicate(pair);
            }
        }

        private void CopyToSerializedList()
        {
            SerializedPairs.Clear();
            foreach (var pair in this)
            {
                SerializedPairs.Add(pair);
            }
        }

        private void CopyFromSerializedList()
        {
            Clear();
            foreach (var serializedPair in SerializedPairs)
            {
                this[serializedPair.Key] = serializedPair.Value;
            }

            SerializedPairs.Clear();
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            CopyFromSerializedList();
        }

        /// <inheritdoc />
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            CopyToSerializedList();
        }

        [Serializable]
        private struct SerializedPair
        {
            /// <summary>
            /// The key of this pair.
            /// </summary>
            [field: SerializeField]
            [field: Tooltip("The key of this pair.")]
            public TKey Key { get; private set; }

            /// <summary>
            /// The value of this pair.
            /// </summary>
            [field: SerializeField]
            [field: Tooltip("The value of this pair.")]
            public TValue Value { get; private set; }

            public SerializedPair(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }

            public static implicit operator SerializedPair(KeyValuePair<TKey, TValue> pair)
            {
                return new SerializedPair(pair.Key, pair.Value);
            }

            public static implicit operator KeyValuePair<TKey, TValue>(SerializedPair pair)
            {
                return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value);
            }
        }
    }
}
#endif